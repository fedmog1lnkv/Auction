<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { apiUrl } from '../api'

const router = useRouter()

const activeTab = ref('active')
const bids = ref([])
const lotsById = ref({})
const errorMessage = ref('')
const isLoading = ref(false)
const now = ref(new Date())

const userId = localStorage.getItem('userId') || ''

let timerId = null

onMounted(() =>{
  if(!localStorage.getItem('token')){
    router.push('/login')
    return
  }
  loadMyBids()

  timerId = setInterval(() => {
    now.value = new Date()
  }, 1000)
})

onUnmounted(() => {
  clearInterval(timerId)
})

const latestBids = computed(() => {
  const result = new Map()

  for (const bid of bids.value) {
    const previous = result.get(bid.lotId)

    if (
      !previous ||
      new Date(bid.createdAt) > new Date(previous.createdAt)
    ) {
      result.set(bid.lotId, bid)
    }
  }

  return Array.from(result.values())
})

const bidRows = computed(() => {
  return latestBids.value
  .map(bid => {
    const lot = lotsById.value[bid.lotId]

    if(!lot){
      return null
    }

    return {
      bid,
      lot,
      isLeading:
        String(lot.currentWinnerId || '').toLowerCase() === userId.toLowerCase()
    }

  })
  .filter(Boolean)
})
 

const visibleRows = computed(() => {
  if(activeTab.value === 'active'){
    return bidRows.value.filter(row => row.lot.status === 'ACTIVE')
  }

  if (activeTab.value === 'won'){
    return bidRows.value.filter(row => {
      return row.lot.status === 'FINISHED' && row.isLeading
    })
  }
  return []
})

async function loadMyBids(){
  const token = localStorage.getItem('token')

  isLoading.value = true
  errorMessage.value = ''

  try{
    const response = await fetch(
      apiUrl('/lots/bids/me?page=1&limit=100'),
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    )

    if (response.status === 401){
      localStorage.removeItem('token')
      router.push('/login')
      return
    }

    if (!response.ok){
      throw new Error ('Не удалось загрузить стоавки')
    }

    const data = await response.json()
    bids.value = data.items || []
    await loadBidLots()
  } catch (error){
    errorMessage.value = error.message
  } finally {
    isLoading.value = false
  }
}

async function loadBidLots() {
  const lotIds = [...new Set(bids.value.map(bid => bid.lotId))]

  const responses = await Promise.all(
    lotIds.map(async lotId => {
      try {
        const response = await fetch(apiUrl(`/lots/${lotId}`))

        if (!response.ok) {
          return null
        }

        return await response.json()
      } catch {
        return null
      }
    })
  )

  lotsById.value = Object.fromEntries(
    responses
      .filter(Boolean)
      .map(lot => [lot.id, lot])
  )
}



function formatPrice(value){
  return `${Number(value || 0).toLocaleString('ru-RU')} ₽`
}

function getTimeLeft(value){
  if(!value){
    return 'Не указано'
  }

  const difference = new Date(value) - now.value

  if(difference <= 0){
    return 'Завершён'
  }

  const hours = Math.floor(difference/ 1000 / 60 / 60)
  const minutes = Math.floor(difference / 1000 / 60) % 60
  const seconds = Math.floor(difference / 1000) % 60

  return `${padTime(hours)}:${padTime(minutes)}:${padTime(seconds)}`
}

function emptyMessage() {
  const message = {
    active: 'Активных ставок пока нет',
    won: 'Выигранных лотов пока нет',
    watching: 'Отслеживаемых лотов пока нет'
  }
  return message[activeTab.value]
}

function padTime(value){
  return String(value).padStart(2, '0')
}

function getBidStatus(row){
  if (row.lot.status === 'FINISHED' && row.isLeading){
    return 'Выигран'
  }
  if (row.lot.status === 'ACTIVE' && row.isLeading) {
    return 'Лидирую'
  }

  if (row.lot.status === 'ACTIVE') {
    return 'Перебили'
  }

  return 'Завершён'
}

function getStatusClass(row){
  return row.isLeading ? 'success' : 'danger'
}


function setTab(tabName) {
  activeTab.value = tabName
}
</script>

<template>
  <main class="page">
    <section class="page-title">
      <h1>Мои ставки</h1>
      <p>Следите за своими ставками и результатами аукционов</p>
    </section>

    <section class="bids-tabs">
      <button
        class="bids-tab"
        :class="{ active: activeTab === 'active' }"
        type="button"
        @click="setTab('active')"
      >
        Активные
      </button>

      <button
        class="bids-tab"
        :class="{ active: activeTab === 'won' }"
        type="button"
        @click="setTab('won')"
      >
        Выигранные
      </button>
    </section>

    <p v-if="isLoading" class="empty-message">
      Загрузка ставок...
    </p>

    <p v-else-if="errorMessage" class="empty-message">
      {{ errorMessage }}
    </p>

    <section v-else class="bids-table-wrap">
      <table class="bids-table">
        <thead>
          <tr>
            <th>Лот</th>
            <th>Статус</th>
            <th>Моя ставка</th>
            <th>Лучшая ставка</th>
            <th>До конца</th>
            <th>Действие</th>
          </tr>
        </thead>

        <tbody v-if="visibleRows.length">
          <tr v-for="row in visibleRows" :key="row.bid.id">
            <td>
              <strong>{{  row.lot.title  }}</strong>
            </td>
            <td>
              <span
                class="status"
                :class="getStatusClass(row)"
              >
              {{ getBidStatus(row) }}
            </span>
            </td>

            <td>{{ formatPrice(row.bid.amount) }}</td>
            <td>{{ formatPrice(row.lot.currentPrice)}}</td>
            <td>{{ getTimeLeft(row.lot.endsAt) }}</td>

            <td>
              <RouterLink
              class="secondary-button"
              :to="`/lots/${row.lot.id}`"
              >
              Открыть
            </RouterLink>
            </td>

          </tr>
        </tbody>
        <tbody v-else>
          <tr>
            <td colspan="6" class="bids-empty-cell">
              {{ emptyMessage() }}
            </td>
          </tr>
        </tbody>
      </table>
    </section>
  </main>
</template>