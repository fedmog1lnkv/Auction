<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useRoute } from 'vue-router'

const API_URL = 'http://186.246.31.83:8080'

const route = useRoute()

const lot = ref(null)
const isLoading = ref(false)
const errorMessage = ref('')
const activePhoto = ref(1)

const now = ref(new Date())
let timerId = null

const lotId = computed(() => route.params.id)

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
    const response = await fetch(`${API_URL}/lots/${lotId.value}`)

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

          <button class="primary-button wide-button" type="button">
            Сделать ставку
          </button>

          <button class="secondary-button wide-button" type="button">
            В избранное
          </button>
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