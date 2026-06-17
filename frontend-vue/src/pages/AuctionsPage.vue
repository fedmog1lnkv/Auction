<script setup>
import { computed, onMounted, ref } from 'vue'

const API_URL = 'http://186.246.31.83:8080'

const lots = ref([])
const isLoading = ref(false)
const errorMessage = ref('')

const selectedStatus = ref('')
const minPrice = ref('')
const maxPrice = ref('')

onMounted(() => {
  loadLots()
})

const filteredLots = computed(() => {
  return lots.value.filter(lot => {
    const price = Number(lot.currentPrice || lot.startingPrice || 0)
    const min = Number(minPrice.value)
    const max = Number(maxPrice.value)

    if (minPrice.value && price < min) {
      return false
    }

    if (maxPrice.value && price > max) {
      return false
    }

    return true
  })
})

async function loadLots() {
  isLoading.value = true
  errorMessage.value = ''

  const params = new URLSearchParams({
    page: '1',
    limit: '20'
  })

  if (selectedStatus.value) {
    params.append('status', selectedStatus.value)
  }

  try {
    const response = await fetch(`${API_URL}/lots?${params.toString()}`)

    if (!response.ok) {
      throw new Error('Не удалось загрузить лоты')
    }

    const data = await response.json()
    lots.value = data.items || []
  } catch (error) {
    errorMessage.value = 'Не удалось загрузить лоты'
  } finally {
    isLoading.value = false
  }
}

function resetFilters() {
  selectedStatus.value = ''
  minPrice.value = ''
  maxPrice.value = ''
  loadLots()
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

        <label class="field">
          <span>Статус</span>
          <select v-model="selectedStatus" @change="loadLots">
            <option value="">Все статусы</option>
            <option value="DRAFT">Черновик</option>
            <option value="ACTIVE">Активный</option>
            <option value="FINISHED">Завершён</option>
            <option value="CANCELLED">Отменён</option>
          </select>
        </label>

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
          <h2>Список лотов</h2>

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
      </section>
    </section>
  </main>
</template>