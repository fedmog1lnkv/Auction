<script setup>

import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { apiUrl } from '../api'

const router = useRouter()

const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const form = ref({
  title: '',
  description: '',
  startingPrice: '',
  minBidStep: '',
  startsAt: '',
  endsAt: ''
})

async function createLot() {
  errorMessage.value = ''
  successMessage.value = ''

  const token = localStorage.getItem('token')

  if(!token){
    router.push('/login')
    return 
  }

  if(!form.value.startsAt || !form.value.endsAt){
    errorMessage.value = 'Укажите дату начала и дату окончания'
    return
  }
  
  if (new Date(form.value.endsAt) <= new Date(form.value.startsAt)){
    errorMessage.value = 'Дата окончания должна быть позже даты начала'
    return
  }

  isLoading.value = true

  const newLot = {
    title: form.value.title,
    description: form.value.description,
    startingPrice: Number(form.value.startingPrice),
    minBidStep: Number(form.value.minBidStep),
    startsAt: new Date(form.value.startsAt).toISOString(),
    endsAt: new Date(form.value.endsAt).toISOString()
  }
  
  try {
  const response = await fetch(apiUrl('/lots'), {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(newLot)
  })

  if (response.status === 401) {
    errorMessage.value = 'Сессия истекла. Войдите заново.'
    localStorage.removeItem('token')
    router.push('/login')
    return
  }

  if (!response.ok) {
    throw new Error('Не удалось создать лот')
  }

  const createdLot = await response.json()
  const lotId = createdLot.id || createdLot.Id

  successMessage.value = 'Лот создан'

  if (lotId) {
    router.push(`/lots/${lotId}`)
  } else {
    router.push('/')
  }
} catch (error) {
  errorMessage.value = error.message
} finally {
  isLoading.value = false
}
}
</script>

<template>
  <main class="page">
    <div class="section-head">
      <div>
        <h1>Создать лот</h1>
        <p class="page-subtitle">Заполните данные товара для аукциона</p>
      </div>

      <RouterLink class="secondary-button" to="/">
        К аукционам
      </RouterLink>
    </div>

    <form class="create-form" @submit.prevent="createLot">
      <label class="field">
        <span>Название лота</span>
        <input
          v-model="form.title"
          type="text"
          placeholder="Например: iPhone 15 Pro"
          required
        >
      </label>

      <label class="field">
        <span>Описание</span>
        <textarea
          v-model="form.description"
          placeholder="Опишите состояние товара, комплект и важные детали"
        ></textarea>
      </label>

      <div class="form-row">
        <label class="field">
          <span>Начальная цена, ₽</span>
          <input
            v-model="form.startingPrice"
            type="number"
            min="1"
            step="1"
            placeholder="1000"
            required
          >
        </label>

        <label class="field">
          <span>Шаг ставки, ₽</span>
          <input
            v-model="form.minBidStep"
            type="number"
            min="1"
            step="1"
            placeholder="100"
            required
          >
        </label>
      </div>

      <div class="form-row">
        <label class="field">
          <span>Дата начала</span>
          <input
            v-model="form.startsAt"
            type="datetime-local"
            required
          >
        </label>

        <label class="field">
          <span>Дата окончания</span>
          <input
            v-model="form.endsAt"
            type="datetime-local"
            required
          >
        </label>
      </div>

      <p v-if="errorMessage" class="auth-error">
        {{ errorMessage }}
      </p>

      <p v-if="successMessage" class="success-message">
        {{ successMessage }}
      </p>

      <div class="form-actions">
        <button class="primary-button" type="submit" :disabled="isLoading">
          {{ isLoading ? 'Создаём...' : 'Создать лот' }}
        </button>

        <RouterLink class="secondary-button" to="/">
          Отмена
        </RouterLink>
      </div>
    </form>
  </main>
</template>
