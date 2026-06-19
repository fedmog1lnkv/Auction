<script setup>

import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { apiUrl } from '../api'

const route = useRoute()
const router = useRouter()

const lots = ref([])
const isLoading = ref(false)
const errorMessage = ref('')

const isLoadingMore = ref('')
const currentPage = ref(1)
const totalLots = ref (0)

const PAGE_SIZE = 20

const minPrice = ref('')
const maxPrice = ref('')
const searchQuery = computed(() => String(route.query.search || ''))

const hasMore = computed(() => {
  return lots.value.length < totalLots.value
})

onMounted(() => {
  loadLots()
})

const filteredLots = computed(() => {
  const query = searchQuery.value.trim().toLowerCase()

  return lots.value.filter(lot => {
    const price = Number(lot.currentPrice || lot.startingPrice || 0)
    const min = Number(minPrice.value)
    const max = Number(maxPrice.value)

    const title = String(lot.title || '').toLowerCase()
    const description = String(lot.description || '').toLowerCase()

    if (query && !title.includes(query) && !description.includes(query)) {
      return false
    }

    if (minPrice.value && price < min) {
      return false
    }

    if (maxPrice.value && price > max) {
      return false
    }

    return true
  })
})

async function loadLots(page = 1, append = false) {

  if(append){
    isLoadingMore.value = true
  } else{
    isLoading.value = true
  }
  errorMessage.value = ''

  const params = new URLSearchParams({
    status: 'ACTIVE',
    page: String(page),
    limit: String(PAGE_SIZE)
  })
  try {
    const response = await fetch(apiUrl(`/lots?${params.toString()}`))

    if (!response.ok) {
      throw new Error('Не удалось загрузить лоты')
    }

    const data = await response.json()
    const newLots = data.items || []

    const combinedLots = append
      ? [...lots.value, ...newLots]
      : newLots

    lots.value = Array.from(
      new Map(combinedLots.map(lot => [lot.id, lot])).values()
    )
    currentPage.value = page
    totalLots.value = Number(data.total || 0)
    } catch (error) {
    errorMessage.value = 'Не удалось загрузить лоты'
  } finally {
    isLoading.value = false
    isLoadingMore.value = false
  }
}

async function loadMore() {
  if (isLoadingMore.value || !hasMore.value){
    return
  }
  await loadLots (currentPage.value + 1, true)
}

function resetFilters() {
  minPrice.value = ''
  maxPrice.value = ''

  router.replace({
    path: '/',
    query: {}
  })

  loadLots(1, false)
}

function formatPrice(value) {
  return `${Number(value || 0).toLocaleString('ru-RU')} ₽`
}

function formatStatus(status) {
  const statuses = {
    DRAFT: 'Черновик',
    ACTIVE: 'Активный',
    FINISHED: 'Завершён',
    CANCELLED: 'Отменён'
  }

  return statuses[status] || status || 'Не указан'
}

function formatDate(value) {
  if (!value) {
    return 'Не указана'
  }

  return new Date(value).toLocaleString('ru-RU')
}
</script>

<template>
  <main class="page">
    <section class="page-title">
      <h1>Аукционы</h1>
      <p>Выберите лот и сделайте ставку</p>
    </section>

    <section class="layout">
      <aside class="filters card">
        <h2>Фильтры</h2>

        <div class="filter-group">
          <span class="filter-title">Цена, ₽</span>

          <div class="price-row">
            <input
              v-model="minPrice"
              type="number"
              min="0"
              placeholder="От"
            >

            <input
              v-model="maxPrice"
              type="number"
              min="0"
              placeholder="До"
            >
          </div>
        </div>

        <button class="secondary-button" type="button" @click="resetFilters">
          Сбросить
        </button>
      </aside>

      <section class="lots-section">
        <div class="section-head">
          <div>
            <h2>Список лотов</h2>
            <p class="muted">
              Показано: {{ filteredLots.length }} из {{ totalLots }}
            </p>
          </div>

          <RouterLink class="primary-button" to="/create-lot">
            Создать лот
          </RouterLink>
        </div>

        <div class="lots-list">
          <p v-if="isLoading" class="empty-message">
            Загрузка лотов...
          </p>

          <p v-else-if="errorMessage" class="empty-message">
            {{ errorMessage }}
          </p>

          <p v-else-if="filteredLots.length === 0" class="empty-message">
            Лоты не найдены
          </p>

          <article
            v-for="lot in filteredLots"
            v-else
            :key="lot.id"
            class="lot-card lot-card-full"
          >
            <div class="lot-card-content">
              <h3>{{ lot.title }}</h3>

              <p class="lot-card-description">
                {{ lot.description || 'Описание не добавлено' }}
              </p>

              <div class="lot-card-row">
                <span>Текущая цена</span>
                <strong>{{ formatPrice(lot.currentPrice) }}</strong>
              </div>

              <div class="lot-card-row">
                <span>Статус</span>
                <strong>{{ formatStatus(lot.status) }}</strong>
              </div>

              <div class="lot-card-row">
                <span>Окончание</span>
                <strong>{{ formatDate(lot.endsAt) }}</strong>
              </div>

              <RouterLink
                class="primary-button lot-card-link"
                :to="`/lots/${lot.id}`"
              >
                Открыть лот
              </RouterLink>
            </div>
          </article>
        </div>
        <div v-if="hasMore" class="load-more-row">
          <button class="secondary-button" type="button" :disabled="isLoadingMore" @click="loadMore">
              {{ isLoadingMore ? 'Загрузка...' : 'Показать ещё' }}
          </button>
        </div>
      </section>
    </section>
  </main>
</template>
