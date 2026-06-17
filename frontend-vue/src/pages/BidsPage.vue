<script setup>
import { ref } from 'vue'

const activeTab = ref('active')

const tabMessages = {
  active: 'Активных ставок пока нет',
  won: 'Выигранных лотов пока нет',
  watching: 'Отслеживаемых лотов пока нет'
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

    <section class="tabs">
      <button
        class="tab"
        :class="{ active: activeTab === 'active' }"
        type="button"
        @click="setTab('active')"
      >
        Активные
      </button>

      <button
        class="tab"
        :class="{ active: activeTab === 'won' }"
        type="button"
        @click="setTab('won')"
      >
        Выигранные
      </button>

      <button
        class="tab"
        :class="{ active: activeTab === 'watching' }"
        type="button"
        @click="setTab('watching')"
      >
        Отслеживаемые
      </button>
    </section>

    <section class="table-card">
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

        <tbody>
          <tr class="empty-row">
            <td colspan="6">
              {{ tabMessages[activeTab] }}
            </td>
          </tr>
        </tbody>
      </table>
    </section>
  </main>
</template>