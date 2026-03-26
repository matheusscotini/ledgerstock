import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '@/views/auth/LoginView.vue'
import RegisterView from '@/views/auth/RegisterView.vue'
import DashboardView from '@/views/dashboard/DashboardView.vue'
import { useAuthStore } from '@/stores/auth'
import ProductsView from '@/views/products/ProductsView.vue'
import MovementsView from '@/views/movements/MovementsView.vue'
import UsersView from '@/views/users/UsersView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/dashboard',
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { public: true },
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView,
      meta: { public: true },
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: DashboardView,
    },
    {
      path: '/products',
      name: 'products',
      component: ProductsView,
    },
    {
      path: '/movements',
      name: 'movements',
      component: MovementsView,
    },
    {
      path: '/users',
      name: 'users',
      component: UsersView,
      meta: { requiresMaster: true },
    },
  ],
})

router.beforeEach(async (to) => {
  const auth = useAuthStore()

  if (auth.token && !auth.user?.roles?.length) {
    auth.hydrate()
    await auth.fetchMe()
  } else if (!auth.user && auth.token) {
    auth.hydrate()
  }

  const isPublicRoute = Boolean(to.meta.public)
  const requiresMaster = Boolean(to.meta.requiresMaster)
  const isMaster = auth.user?.roles?.includes('Master')

  if (!isPublicRoute && !auth.isAuthenticated) {
    return '/login'
  }

  if (isPublicRoute && auth.isAuthenticated) {
    return '/dashboard'
  }

  if (requiresMaster && !isMaster) {
    return '/dashboard'
  }

  return true
})

export default router