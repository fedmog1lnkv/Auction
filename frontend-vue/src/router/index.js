import { createRouter, createWebHistory } from 'vue-router'

import AuctionsPage from '../pages/AuctionsPage.vue'
import LotPage from '../pages/LotPage.vue'
import LoginPage from '../pages/LoginPage.vue'
import BidsPage from '../pages/BidsPage.vue'
import CreateLotPage from '../pages/CreateLotPage.vue'
import HowItWorksPage from '../pages/HowItWorksPage.vue'

const routes = [
  {
    path: '/',
    name: 'auctions',
    component: AuctionsPage
  },
  {
    path: '/lots/:id',
    name: 'lot',
    component: LotPage
  },
  {
    path: '/login',
    name: 'login',
    component: LoginPage
  },
  {
    path: '/bids',
    name: 'bids',
    component: BidsPage
  },
  {
    path: '/create-lot',
    name: 'create-lot',
    component: CreateLotPage
  },
  {
  path: '/how-it-works',
  name: 'how-it-works',
  component: HowItWorksPage
  }
]   

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router