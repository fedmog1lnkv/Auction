<script setup>
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const router = useRouter()
const route = useRoute()
const searchQuery = ref(String(route.query.search || ''))

watch(
  () => route.query.search,
  value => {
    searchQuery.value = String(value || '')
  }
)

function searchLots() {
  router.replace({
    path: '/',
    query: searchQuery.value.trim()
      ? { search: searchQuery.value.trim() }
      : {}
  })
}

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

      <input
        v-model="searchQuery"
        class="header-search"
        type="text"
        placeholder="Поиск лотов"
        @input="searchLots"
      >

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