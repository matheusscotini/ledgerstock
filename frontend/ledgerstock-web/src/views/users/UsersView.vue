<template>
  <AppLayout>
    <div class="users-page">
      <div class="users-header card">
        <div class="users-header__content">
          <div class="users-header__icon">
            <Users :size="22" />
          </div>

          <div>
            <h2>Gestão de usuários</h2>
            <p>Gerencie acessos e perfis do sistema. Área exclusiva para Master.</p>
          </div>
        </div>

        <button class="btn btn-primary users-header__button" @click="showModal = true">
          <Plus :size="16" />
          <span>Novo usuário</span>
        </button>
      </div>

      <div class="users-table-section card">
        <div v-if="loading" class="empty-state">
          <strong>Carregando usuários</strong>
          <p>Aguarde enquanto buscamos os dados.</p>
        </div>

        <div v-else-if="!users.length" class="empty-state">
          <strong>Nenhum usuário encontrado</strong>
          <p>Crie o primeiro usuário para começar o controle de acessos.</p>
        </div>

        <div v-else class="users-table-wrapper">
          <table class="users-table">
            <thead>
              <tr>
                <th>Nome</th>
                <th>E-mail</th>
                <th>Perfil</th>
                <th>Ações</th>
              </tr>
            </thead>

            <tbody>
              <tr v-for="user in paginatedUsers" :key="user.id">
                <td>{{ user.fullName }}</td>
                <td>{{ user.email }}</td>
                <td>
                  <span
                    class="status-badge"
                    :class="getRoleBadgeClass(user.roles?.[0])"
                  >
                    {{ getRoleLabel(user.roles?.[0]) }}
                  </span>
                </td>
                <td>
                  <div class="users-actions">
                    <button
                      class="users-link users-link--danger"
                      @click="handleDelete(user)"
                      :disabled="user.roles?.includes('Master')"
                    >
                      Excluir
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div v-if="users.length" class="pagination">
        <button
          class="btn btn-secondary pagination__button"
          :disabled="currentPage === 1"
          @click="goToPreviousPage"
        >
          Anterior
        </button>

        <div class="pagination__info">
          Página <strong>{{ currentPage }}</strong> de <strong>{{ totalPages }}</strong>
        </div>

        <button
          class="btn btn-secondary pagination__button"
          :disabled="currentPage === totalPages"
          @click="goToNextPage"
        >
          Próxima
        </button>
      </div>

      <UserModal
        v-if="showModal"
        :loading="modalLoading"
        @close="closeModal"
        @submit="handleCreateUser"
      />
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import AppLayout from '@/layouts/AppLayout.vue'
import UserModal from '@/components/ui/UserModal.vue'
import { createUserRequest, deleteUserRequest, getUsersRequest } from '@/api/users'
import { useUiStore } from '@/stores/ui'
import { Users, Plus } from 'lucide-vue-next'

const ui = useUiStore()

const loading = ref(false)
const modalLoading = ref(false)
const showModal = ref(false)
const users = ref<any[]>([])
const currentPage = ref(1)
const itemsPerPage = 8

const totalPages = computed(() => {
  return Math.max(1, Math.ceil(users.value.length / itemsPerPage))
})

const paginatedUsers = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return users.value.slice(start, end)
})

const loadUsers = async () => {
  loading.value = true

  try {
    const response = await getUsersRequest()
    users.value = response
    if (currentPage.value > totalPages.value) {
      currentPage.value = 1
    }
  } finally {
    loading.value = false
  }
}

const handleCreateUser = async (payload: any) => {
  modalLoading.value = true

  try {
    await createUserRequest(payload)
    ui.showToast('success', 'Usuário criado com sucesso.')
    closeModal()
    await loadUsers()
  } catch (error: any) {
    ui.showToast('error', error?.response?.data?.message || 'Não foi possível criar o usuário.')
  } finally {
    modalLoading.value = false
  }
}

const handleDelete = async (user: any) => {
  if (user.roles?.includes('Master')) {
    ui.showToast('error', 'Não é permitido excluir um usuário Master.')
    return
  }

  const confirmed = window.confirm(`Deseja realmente excluir o usuário ${user.fullName}?`)
  if (!confirmed) return

  try {
    await deleteUserRequest(user.id)
    ui.showToast('success', 'Usuário removido com sucesso.')
    await loadUsers()
  } catch (error: any) {
    ui.showToast('error', error?.response?.data?.message || 'Não foi possível excluir o usuário.')
  }
}

const closeModal = () => {
  showModal.value = false
}

const goToPreviousPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

const goToNextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

const getRoleLabel = (role?: string) => {
  if (role === 'Master') return 'Master'
  if (role === 'Admin') return 'Admin'
  return 'Padrão'
}

const getRoleBadgeClass = (role?: string) => {
  if (role === 'Master') return 'status-badge--danger'
  if (role === 'Admin') return 'status-badge--warning'
  return 'status-badge--success'
}

onMounted(() => {
  loadUsers()
})
</script>

<style scoped>
.users-page {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.users-header,
.users-table-section {
  padding: 24px;
}

.users-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
}

.users-header__content {
  display: flex;
  align-items: center;
  gap: 16px;
}

.users-header__icon {
  width: 50px;
  height: 50px;
  border-radius: 16px;
  background: rgba(139, 92, 246, 0.12);
  color: #7c3aed;
  display: grid;
  place-items: center;
}

.users-header h2 {
  font-size: 24px;
  margin-bottom: 6px;
}

.users-header p {
  color: var(--color-text-secondary);
}

.users-header__button {
  display: inline-flex;
  align-items: center;
  gap: 8px;
}

.users-table-wrapper {
  overflow-x: auto;
}

.users-table {
  width: 100%;
  border-collapse: collapse;
}

.users-table th,
.users-table td {
  padding: 15px 12px;
  border-bottom: 1px solid var(--color-border);
  text-align: left;
  font-size: 14px;
}

.users-table th {
  color: var(--color-text-secondary);
  font-weight: 700;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.users-table tbody tr:hover {
  background: rgba(37, 99, 235, 0.03);
}

.users-actions {
  display: flex;
  gap: 12px;
}

.users-link {
  font-weight: 700;
}

.users-link--danger {
  color: var(--color-danger);
}

.users-link:disabled {
  opacity: 0.45;
  cursor: not-allowed;
}

.pagination {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  padding-top: 20px;
  border-top: 1px solid var(--color-border);
}

.pagination__info {
  font-size: 14px;
  color: var(--color-text-secondary);
}

.pagination__button {
  min-width: 110px;
}

@media (max-width: 780px) {
  .users-header {
    flex-direction: column;
    align-items: stretch;
  }
}
</style>