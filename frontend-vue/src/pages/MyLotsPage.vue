<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { apiUrl } from '../api'

const router = useRouter()

const lots = ref([])
const isLoading = ref(false)
const errorMessage = ref('')
const actionMessage = ref('')
const actionError = ref('')
const actionLotId = ref('')

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
    const response = await fetch(
      apiUrl(`/lots?seller_id=${userId}&page=1&limit=20`)
    )

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

async function performLotAction(lot, action) {
  const token = localStorage.getItem('token')

  if (!token) {
    router.push('/login')
    return
  }

  const confirmations = {
    finish: 'Завершить этот аукцион?',
    cancel: 'Отменить этот лот?',
    delete: 'Удалить этот лот без возможности восстановления?'
  }

  if (confirmations[action] && !window.confirm(confirmations[action])) {
    return
  }

  actionMessage.value = ''
  actionError.value = ''
  actionLotId.value = lot.id

  const endpoint = action === 'delete'
    ? `/lots/${lot.id}`
    : `/lots/${lot.id}/${action}`

  const method = action === 'delete' ? 'DELETE' : 'POST'

  try {
    const response = await fetch(apiUrl(endpoint), {
      method,
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    const data = await response.json().catch(() => null)

    if (response.status === 401) {
      localStorage.removeItem('token')
      router.push('/login')
      return
    }

    if (!response.ok) {
      throw new Error(data?.message || 'Не удалось изменить лот')
    }

    const messages = {
      start: 'Лот запущен',
      finish: 'Аукцион завершён',
      cancel: 'Лот отменён',
      delete: 'Лот удалён'
    }

    actionMessage.value = messages[action]
    await loadMyLots()
  } catch (error) {
    actionError.value = error.message
  } finally {
    actionLotId.value = ''
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

    <p v-if="actionMessage" class="success-message">
      {{ actionMessage }}
    </p>

    <p v-if="actionError" class="auth-error">
      {{ actionError }}
    </p>

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

          <div class="my-lot-actions">
            <RouterLink
              class="secondary-button"
              :to="`/lots/${lot.id}`"
            >
              Открыть
            </RouterLink>

            <button
              v-if="lot.status === 'DRAFT'"
              class="primary-button"
              type="button"
              :disabled="actionLotId === lot.id"
              @click="performLotAction(lot, 'start')"
            >
              Запустить
            </button>

            <button
              v-if="lot.status === 'ACTIVE'"
              class="primary-button"
              type="button"
              :disabled="actionLotId === lot.id"
              @click="performLotAction(lot, 'finish')"
            >
              Завершить
            </button>

            <button
              v-if="lot.status === 'DRAFT' || lot.status === 'ACTIVE'"
              class="danger-button"
              type="button"
              :disabled="actionLotId === lot.id"
              @click="performLotAction(lot, 'cancel')"
            >
              Отменить
            </button>

            <button
              v-if="lot.status === 'DRAFT'"
              class="danger-button"
              type="button"
              :disabled="actionLotId === lot.id"
              @click="performLotAction(lot, 'delete')"
            >
              Удалить
            </button>
          </div>
        </div>
      </article>
    </section>
  </main>
</template>