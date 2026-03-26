import { defineStore } from 'pinia'

export interface ToastItem {
  id: number
  type: 'success' | 'error' | 'info'
  message: string
}

export const useUiStore = defineStore('ui', {
  state: () => ({
    toasts: [] as ToastItem[],
  }),

  actions: {
    showToast(type: ToastItem['type'], message: string) {
      const id = Date.now() + Math.floor(Math.random() * 1000)

      this.toasts.push({ id, type, message })

      setTimeout(() => {
        this.toasts = this.toasts.filter((toast) => toast.id !== id)
      }, 3500)
    },
  },
})