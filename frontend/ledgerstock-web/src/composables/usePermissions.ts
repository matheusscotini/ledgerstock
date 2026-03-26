import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

export const usePermissions = () => {
  const auth = useAuthStore()

  const roles = computed(() => auth.user?.roles || [])

  const isMaster = computed(() => roles.value.includes('Master'))
  const isAdmin = computed(() => roles.value.includes('Admin'))
  const isStandard = computed(() => roles.value.includes('Standard'))

  const canManageUsers = computed(() => isMaster.value)
  const canManageProducts = computed(() => isMaster.value || isAdmin.value)
  const canManageMovements = computed(() => isMaster.value || isAdmin.value)
  const canViewData = computed(() => auth.isAuthenticated)

  return {
    roles,
    isMaster,
    isAdmin,
    isStandard,
    canManageUsers,
    canManageProducts,
    canManageMovements,
    canViewData,
  }
}