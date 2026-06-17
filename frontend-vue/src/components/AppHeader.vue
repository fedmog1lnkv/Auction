<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const isAuth = computed(() => Boolean(localStorage.getItem('token')))

const userName = computed(() => {
  const firstName = localStorage.getItem('firstName') || ''
  const lastName = localStorage.getItem('lastName') || ''
  const email = localStorage.getItem('email') || ''

  const fullName = `${firstName} ${lastName}`.trim()
  return fullName || email || 'Профиль'
})

const userInitials = computed(() => {
  const name = userName.value

  if (name.includes('@')) {
    return name[0]?.toUpperCase() || 'П'
  }

  return name
    .split(' ')
    .map(part => part[0])
    .join('')
    .slice(0, 2)
    .toUpperCase()
})

function logout() {
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
  <header class="app-header">
    <div class="app-header-inner">
      <RouterLink class="header-logo" to="/">
        <span class="header-logo-icon">⚖</span>
        <span>Аукцион</span>
      </RouterLink>

      <nav class="header-nav">
        <RouterLink to="/">Аукционы</RouterLink>
        <RouterLink to="/bids">Мои ставки</RouterLink>
        <RouterLink to="/how-it-works">Как это работает</RouterLink>
      </nav>

      <input class="header-search" type="text" placeholder="Поиск лотов">

      <div class="header-profile">
        <RouterLink v-if="isAuth" class="profile-link" to="/profile">
          <span class="profile-avatar">{{ userInitials }}</span>
          <span class="profile-name">{{ userName }}</span>
        </RouterLink>
        <RouterLink v-else class="login-link" to="/login">
          Войти
        </RouterLink>
      </div>
    </div>
  </header>
</template>