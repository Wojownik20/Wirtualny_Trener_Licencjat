import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import DashboardView from '../views/DashboardView.vue'
import PlayersView from '../views/PlayersView.vue'
import FixturesView from '../views/FixturesView.vue'
import StatisticsView from '../views/StatisticsView.vue'
import ProfileView from '../views/ProfileView.vue'
import FormationsView from '../views/FormationsView.vue'

const routes = [
  { path: '/', component: LoginView },
  { path: '/dashboard', component: DashboardView },
  { path: '/players', component: PlayersView },
  { path: '/fixtures', component: FixturesView },
  { path: '/statistics', component: StatisticsView},
  { path: '/profile', component: ProfileView},
  {path: '/formations', component: FormationsView}

]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const token = sessionStorage.getItem('token')

  if (to.path !== '/' && !token) {
    return next('/')
  }

  if (to.path === '/' && token) {
    return next('/dashboard')
  }

  next()
})

export default router

