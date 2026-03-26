import { defineStore } from 'pinia'
import { getMeRequest, loginRequest, registerRequest } from '@/api/auth'

interface User {
  id: string
  fullName: string
  email: string
  roles: string[]
}

interface LoginPayload {
  email: string
  password: string
}

interface RegisterPayload {
  fullName: string
  email: string
  password: string
  confirmPassword: string
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null as User | null,
    token: localStorage.getItem('ledgerstock_token') || '',
    isAuthenticated: !!localStorage.getItem('ledgerstock_token'),
    loading: false,
  }),

  actions: {
    async login(payload: LoginPayload) {
      this.loading = true

      try {
        const response = await loginRequest(payload)

        this.token = response.token
        this.user = response.user
        this.isAuthenticated = true

        localStorage.setItem('ledgerstock_token', response.token)
        localStorage.setItem('ledgerstock_user', JSON.stringify(response.user))

        return response
      } finally {
        this.loading = false
      }
    },

    async register(payload: RegisterPayload) {
      this.loading = true

      try {
        const response = await registerRequest(payload)

        this.token = response.token
        this.user = response.user
        this.isAuthenticated = true

        localStorage.setItem('ledgerstock_token', response.token)
        localStorage.setItem('ledgerstock_user', JSON.stringify(response.user))

        return response
      } finally {
        this.loading = false
      }
    },

    async fetchMe() {
      if (!this.token) return

      try {
        const user = await getMeRequest()
        this.user = user
        this.isAuthenticated = true
        localStorage.setItem('ledgerstock_user', JSON.stringify(user))
      } catch {
        this.logout()
      }
    },

    hydrate() {
      const storedUser = localStorage.getItem('ledgerstock_user')
      const storedToken = localStorage.getItem('ledgerstock_token')

      if (storedToken) {
        this.token = storedToken
        this.isAuthenticated = true
      }

      if (storedUser) {
        this.user = JSON.parse(storedUser)
      }
    },

    logout() {
      this.user = null
      this.token = ''
      this.isAuthenticated = false

      localStorage.removeItem('ledgerstock_token')
      localStorage.removeItem('ledgerstock_user')
    },
  },
})