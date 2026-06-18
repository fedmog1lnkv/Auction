<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { apiUrl } from '../api'

const route = useRoute()

const lot = ref(null)
const isLoading = ref(false)
const errorMessage = ref('')
const activePhoto = ref(1)

const actionMessage = ref('')
const actionError = ref('')
const isActionLoading = ref(false)

const userId = localStorage.getItem('userId') || ''
const bidAmount = ref('')

const now = ref(new Date())
let timerId = null

const lotId = computed(() => route.params.id)


const isOwner = computed(() => {
  return String(lot.value?.sellerId || '').toLowerCase() === userId.toLowerCase()
})

const canStartLot = computed(() => {
  return isOwner.value && lot.value?.status === 'DRAFT'
})

const minimumBidAmount = computed(() =>{
  return Number(lot.value?.currentPrice || 0) +
    Number(lot.value?.minBidStep || 0)
})

const canPlaceBid = computed(() =>{
  if (!lot.value || isOwner.value){
    return false
  }

  return lot.value.status === "ACTIVE" && 
    new Date(lot.value.endsAt) > now.value

})

onMounted(() => {
  loadLot()

  timerId = setInterval(() => {
    now.value = new Date()
  }, 1000)
})

onUnmounted(() => {
  clearInterval(timerId)
})

async function loadLot() {
  if (!lotId.value) {
    errorMessage.value = 'Лот не выбран'
    return
  }

  isLoading.value = true
  errorMessage.value = ''

  try {
    const response = await fetch(apiUrl(`/lots/${lotId.value}`))

    if (!response.ok) {
      throw new Error('Не удалось загрузить лот')
    }

    lot.value = await response.json()
  } catch (error) {
    errorMessage.value = 'Не удалось загрузить лот'
  } finally {
    isLoading.value = false
  }
}

function formatPrice(value) {
  return `${Number(value || 0).toLocaleString('ru-RU')} ₽`
}
async function startLot() {
  const token = localStorage.getItem('token')

  if (!token) {
    actionError.value = 'Сначала войдите в аккаунт'
    return
  }

  actionMessage.value = ''
  actionError.value = ''
  isActionLoading.value = true

  try {
    const response = await fetch(apiUrl(`/lots/${lotId.value}/start`), {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    if (response.status === 401) {
      throw new Error('Сессия истекла. Войдите заново.')
    }

    if (!response.ok) {
      throw new Error('Не удалось запустить лот')
    }

    actionMessage.value = 'Лот успешно запущен'
    await loadLot()
  } catch (error) {
    actionError.value = error.message
  } finally {
    isActionLoading.value = false
  }
}

async function placeBid(){
  const token = localStorage.getItem('token')
  const amount = Number(bidAmount.value)

  actionMessage.value = ''
  actionError.value = ''

  if(!token){
    actionError.value = 'Сначала войдите в аккаунт'
  }

  if(!Number.isFinite(amount) || amount <= 0){
    actionError.value = `Минимальная ставка: ${formatPrice(minimumBidAmount.value)}`
    return
  }

  isActionLoading.value = true

  try{
    const response = await fetch(
      apiUrl(`/lots/${lotId.value}/bids`),
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({
          amount
        })
      }
    )
  
    const data  = await response.json().catch(() => null)
    
    if (!response.ok){
      throw new Error(data?.message || 'Не удалось сделать ставку')
    }

    actionMessage.value = 'Cnfdrf ecgtiyj ghbyzndf'
    bidAmount.value = ''

    await loadLot()
  } catch(error){
    actionError.value = error.message
  } finally{
    isActionLoading.value = false
  }
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

function getTimeLeft(value) {
  if (!value) {
    return '--:--:--'
  }

  const endDate = new Date(value)
  const diff = endDate - now.value

  if (diff <= 0) {
    return 'Завершён'
  }

  const hours = Math.floor(diff / 1000 / 60 / 60)
  const minutes = Math.floor(diff / 1000 / 60) % 60
  const seconds = Math.floor(diff / 1000) % 60

  return `${padTime(hours)}:${padTime(minutes)}:${padTime(seconds)}`
}

function padTime(value) {
  return String(value).padStart(2, '0')
}
</script>

<template>
  <main class="page">
    <div class="breadcrumbs">
      <RouterLink to="/">Главная</RouterLink>
      <span>/</span>
      <RouterLink to="/">Аукционы</RouterLink>
      <span>/</span>
      <span>{{ lot ? lot.title : 'Карточка лота' }}</span>
    </div>

    <p v-if="isLoading" class="empty-message">
      Загрузка лота...
    </p>

    <p v-else-if="errorMessage" class="empty-message">
      {{ errorMessage }}
    </p>

    <template v-else-if="lot">
      <section class="lot-details-layout">
        <section class="product-info">
          <h1>{{ lot.title || 'Без названия' }}</h1>

          <p class="lot-label">Текущая ставка</p>
          <p class="lot-price">{{ formatPrice(lot.currentPrice) }}</p>

          <p class="lot-label">Шаг ставки</p>
          <p class="lot-price small">{{ formatPrice(lot.minBidStep) }}</p>

          <p class="lot-label">До окончания</p>
          <p class="lot-time">{{ getTimeLeft(lot.endsAt) }}</p>

          <button
              v-if="canStartLot"
              class="primary-button wide-button"
              type="button"
              :disabled="isActionLoading"
              @click="startLot"
              >
              {{ isActionLoading ? 'Запускаем...' : 'Запустить лот' }}
            </button>

            <form
              v-else-if="canPlaceBid"
              class="bid-form"
              @submit.prevent="placeBid"
            >
              <label class="field">
                <span>Сумма ставки, ₽</span>

                <input
                  v-model="bidAmount"
                  type="number"
                  step="0.01"
                  :min="minimumBidAmount"
                  :placeholder="String(minimumBidAmount)"
                  required
                >
              </label>

              <p class="bid-minimum">
                Минимальная ставка:
                <strong>{{ formatPrice(minimumBidAmount) }}</strong>
              </p>

              <button
                class="primary-button wide-button"
                type="submit"
                :disabled="isActionLoading"
              >
                {{ isActionLoading ? 'Отправляем...' : 'Сделать ставку' }}
              </button>
            </form>

            <p v-else-if="isOwner" class="muted">
              Вы продавец этого лота и не можете делать ставки.
            </p>

            <p v-else-if="lot.status !== 'ACTIVE'" class="muted">
              Ставки доступны только для активных лотов.
            </p>

            <p v-else class="muted">
              Приём ставок завершён.
            </p>

            <p v-if="actionMessage" class="success-message">
                {{ actionMessage }}
            </p>

            <p v-if="actionError" class="auth-error">
                {{ actionError }}
            </p>
        </section>

        <section class="product-gallery">
          <div class="main-product-image">
            <div class="image-placeholder">
              Фото {{ activePhoto }}
            </div>
          </div>

          <div class="thumbnail-row">
            <button
              v-for="photoNumber in 4"
              :key="photoNumber"
              class="thumbnail"
              :class="{ active: activePhoto === photoNumber }"
              type="button"
              @click="activePhoto = photoNumber"
            >
              {{ photoNumber }}
            </button>
          </div>
        </section>

        <aside class="bid-history">
          <h2>История ставок</h2>

          <div class="bid-list">
            <p class="empty-message">
              История ставок пока недоступна
            </p>
          </div>
        </aside>
      </section>

      <section class="product-extra">
        <div class="description-block">
          <h2>Описание</h2>
          <p>
            {{ lot.description || 'Описание не добавлено' }}
          </p>
        </div>

        <div class="characteristics-block">
          <h2>Характеристики</h2>

          <table class="characteristics-table">
            <tbody>
              <tr>
                <td>Статус</td>
                <td>{{ formatStatus(lot.status) }}</td>
              </tr>
              <tr>
                <td>Начальная цена</td>
                <td>{{ formatPrice(lot.startingPrice) }}</td>
              </tr>
              <tr>
                <td>Продавец</td>
                <td>{{ lot.sellerId || 'Не указан' }}</td>
              </tr>
              <tr>
                <td>Дата начала</td>
                <td>{{ formatDate(lot.startsAt) }}</td>
              </tr>
              <tr>
                <td>Дата окончания</td>
                <td>{{ formatDate(lot.endsAt) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
    </template>
  </main>
</template>
