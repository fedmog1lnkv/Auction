<script setup>
import { ref, onMounted } from 'vue'

const API_URL = 'http://186.246.31.83:8080'

const lots = ref([])
const isLoading = ref(false)
const errorMessage = ref('')

onMounted(() => {
  loadLots()
})

async function loadLots() {
  isLoading.value = true
  errorMessage.value = ''

  try {
    const response = await fetch(`${API_URL}/lots?page=1&limit=20`)

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
      <aside class="filters">
        <h2>Фильтры</h2>

        <label class="field">
          <span>Статус</span>
          <select>
            <option value="">Все статусы</option>
            <option value="DRAFT">Черновик</option>
            <option value="ACTIVE">Активный</option>
            <option value="FINISHED">Завершён</option>
            <option value="CANCELLED">Отменён</option>
          </select>
        </label>

        <label class="field">
          <span>Цена от</span>
          <input type="number" placeholder="0">
        </label>

        <label class="field">
          <span>Цена до</span>
          <input type="number" placeholder="100000">
        </label>

        <button class="secondary-button" type="button">
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

          <p v-else-if="lots.length === 0" class="empty-message">
            Лотов пока нет
          </p>

          <article
            v-for="lot in lots"
            v-else
            :key="lot.id"
            class="lot-card"
          >
            <div class="lot-card-content">
              <h3>{{ lot.title }}</h3>

              <p class="muted">ID: {{ lot.id }}</p>

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