<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { apiUrl } from '../api'

const router = useRouter()

const activeTab = ref('login')
const isLoading = ref(false)
const errorMessage = ref('')

const loginData = ref({
  email: '',
  password: ''
})

const registerData = ref({
  firstName: '',
  lastName: '',
  email: '',
  password: ''
})

function switchTab(tab) {
  activeTab.value = tab
  errorMessage.value = ''
}

async function login() {
  errorMessage.value = ''
  isLoading.value = true

  try {
    const response = await fetch(apiUrl('/api/auth/login'), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(loginData.value)
    })

    if (!response.ok) {
      throw new Error('Не удалось войти. Проверьте email и пароль.')
    }

    const data = await response.json()
    saveUser(data)

    router.push('/')
  } catch (error) {
    errorMessage.value = error.message
  } finally {
    isLoading.value = false
  }
}

async function register() {
  errorMessage.value = ''
  isLoading.value = true

  try {
    const response = await fetch(apiUrl('/api/auth/register'), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(registerData.value)
    })

    if (!response.ok) {
      throw new Error('Не удалось зарегистрироваться.')
    }

    const data = await response.json()
    saveUser(data)

    router.push('/')
  } catch (error) {
    errorMessage.value = error.message
  } finally {
    isLoading.value = false
  }
}

function saveUser(data) {
  localStorage.setItem('token', data.accessToken || data.AccessToken || '')
  localStorage.setItem('refreshToken', data.refreshToken || data.RefreshToken || '')
  localStorage.setItem('userId', data.userId || data.UserId || '')
  localStorage.setItem('email', data.email || data.Email || '')
  localStorage.setItem('firstName', data.firstName || data.FirstName || '')
  localStorage.setItem('lastName', data.lastName || data.LastName || '')
}
</script>

<template>
  <main class="auth-page">
    <section class="auth-card">
      <RouterLink class="auth-back" to="/">
        ← К аукционам
      </RouterLink>

      <div class="auth-head">
        <h1>Вход</h1>
        <p>Войдите в аккаунт или зарегистрируйтесь</p>
      </div>

      <div class="auth-tabs">
        <button
          class="auth-tab"
          :class="{ active: activeTab === 'login' }"
          type="button"
          @click="switchTab('login')"
        >
          Войти
        </button>

        <button
          class="auth-tab"
          :class="{ active: activeTab === 'register' }"
          type="button"
          @click="switchTab('register')"
        >
          Регистрация
        </button>
      </div>

      <form v-if="activeTab === 'login'" class="auth-form" @submit.prevent="login">
        <label class="field">
          <span>Email</span>
          <input v-model="loginData.email" type="email" required>
        </label>

        <label class="field">
          <span>Пароль</span>
          <input v-model="loginData.password" type="password" required>
        </label>

        <button class="primary-button" type="submit" :disabled="isLoading">
          {{ isLoading ? 'Входим...' : 'Войти' }}
        </button>
      </form>

      <form v-else class="auth-form" @submit.prevent="register">
        <label class="field">
          <span>Имя</span>
          <input v-model="registerData.firstName" type="text" required>
        </label>

        <label class="field">
          <span>Фамилия</span>
          <input v-model="registerData.lastName" type="text" required>
        </label>

        <label class="field">
          <span>Email</span>
          <input v-model="registerData.email" type="email" required>
        </label>

        <label class="field">
          <span>Пароль</span>
          <input v-model="registerData.password" type="password" required>
        </label>

        <button class="primary-button" type="submit" :disabled="isLoading">
          {{ isLoading ? 'Создаём аккаунт...' : 'Создать аккаунт' }}
        </button>
      </form>

      <p v-if="errorMessage" class="auth-error">
        {{ errorMessage }}
      </p>
    </section>
  </main>
</template>
