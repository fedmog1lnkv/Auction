<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'

const API_URL = 'http://186.246.31.83:8080'

const router = useRouter()

const lots = ref([])
const isLoading = ref(false)
const errorMessage = ref('')

const userId = localStorage.getItem('userId') || ''

onMounted(() => {
  if (!userId) {
    router.push('/login')
    return
  }

  loadMyLots()
})

async function loadMyLots() {
  isLoading.value = true
  errorMessage.value = ''

  try {
    const response = await fetch(`${API_URL}/lots?seller_id=${userId}&page=1&limit=20`)

    if (!response.ok) {
      throw new Error('Не удалось загрузить ваши лоты')
    }

    const data = await response.json()
    lots.value = data.items || []
  } catch (error) {
    errorMessage.value = error.message
  } finally {
    isLoading.value = false
  }
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
    <div class="section-head">
      <div>
        <h1>Мои лоты</h1>
        <p class="page-subtitle">Лоты, которые вы создали</p>
      </div>

      <RouterLink class="primary-button" to="/create-lot">
        Создать лот
      </RouterLink>
    </div>

    <p v-if="isLoading" class="empty-message">
      Загрузка лотов...
    </p>

    <p v-else-if="errorMessage" class="empty-message">
      {{ errorMessage }}
    </p>

    <p v-else-if="lots.length === 0" class="empty-message">
      Вы пока не создали ни одного лота
    </p>

    <section v-else class="lots-list">
      <article
        v-for="lot in lots"
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
    </section>
  </main>
</template>