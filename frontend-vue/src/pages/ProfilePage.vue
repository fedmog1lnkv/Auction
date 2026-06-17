<script setup>

import { computed } from 'vue'
import { useRouter } from 'vue-router';


const router = useRouter()

const firstName = localStorage.getItem('firstName') || ''
const lastName = localStorage.getItem('lastName') || ''
const email = localStorage.getItem('email') || ''
const userId = localStorage.getItem('userId') || ''

const fullName = computed(() =>{
    const name = `${firstName} ${lastName}`.trim()
    return name || 'Пользователь'
})

const initials = computed(() =>{
    return fullName.value
        .split(' ')
        .map(part => part[0])
        .join('')
        .slice(0, 2)
        .toUpperCase()
})

function logout(){
    localStorage.removeItem('token')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('userId')
    localStorage.removeItem('email')
    localStorage.removeItem('firstName')
    localStorage.removeItem('lastName')

    router.push('/login')
}
</script>

<template>
  <main class="page">
    <section class="profile-card card">
      <div class="profile-main">
        <div class="profile-avatar-large">
          {{ initials }}
        </div>

        <div>
          <h1>{{ fullName }}</h1>
          <p class="profile-email">{{ email || 'Email не указан' }}</p>
        </div>
      </div>

      <div class="profile-info">
        <div class="profile-info-row">
          <span>Имя</span>
          <strong>{{ firstName || 'Не указано' }}</strong>
        </div>

        <div class="profile-info-row">
          <span>Фамилия</span>
          <strong>{{ lastName || 'Не указана' }}</strong>
        </div>

        <div class="profile-info-row">
          <span>Email</span>
          <strong>{{ email || 'Не указан' }}</strong>
        </div>

        <div class="profile-info-row">
          <span>ID пользователя</span>
          <strong>{{ userId || 'Не указан' }}</strong>
        </div>
      </div>

      <div class="profile-actions">
        <RouterLink class="secondary-button" to="/bids">
          Мои ставки
        </RouterLink>

        <RouterLink class="secondary-button" to="/my-lots">
            Мои лоты
        </RouterLink>

        <button class="primary-button" type="button" @click="logout">
          Выйти
        </button>
      </div>
    </section>
  </main>
</template>