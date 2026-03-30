<template>
  <AppLayout>
    <div class="movements-page">
      <div class="movements-header card">
        <div class="movements-header__content">
          <div class="movements-header__icon">
            <ArrowLeftRight :size="22" />
          </div>

          <div>
            <h2>Movimentações de estoque</h2>
            <p>Registre entradas e saídas e acompanhe o histórico operacional.</p>
          </div>
        </div>

        <div class="movements-header__actions">
          <button class="btn btn-secondary movements-header__button" @click="handleExportCsv">
            <Download :size="16" />
            <span>Exportar CSV</span>
          </button>

          <button
            v-if="canManageMovements"
            class="btn btn-primary movements-header__button"
            @click="showModal = true"
          >
            <Plus :size="16" />
            <span>Nova movimentação</span>
          </button>
        </div>
      </div>

      <div class="movements-filters card">
        <div class="movements-filters__grid">
          <div class="movements-filter-field">
            <label>Produto</label>
            <select v-model="filters.productId" class="input">
              <option value="">Todos os produtos</option>
              <option v-for="product in products" :key="product.id" :value="product.id">
                {{ product.name }} — {{ product.sku }}
              </option>
            </select>
          </div>

          <div class="movements-filter-field">
            <label>Tipo</label>
            <select v-model="filters.type" class="input">
              <option value="">Todos os tipos</option>
              <option value="1">Entrada</option>
              <option value="2">Saída</option>
            </select>
          </div>

          <div class="movements-filter-field">
            <label>Data inicial</label>
            <input v-model="filters.startDate" class="input" type="date" />
          </div>

          <div class="movements-filter-field">
            <label>Data final</label>
            <input v-model="filters.endDate" class="input" type="date" />
          </div>
        </div>

        <button class="btn btn-secondary" @click="loadMovements">Filtrar</button>
      </div>

      <div class="movements-table-section card">
        <div v-if="loading" class="movements-empty">Carregando movimentações...</div>

        <div v-else-if="!movements.length" class="empty-state">
          <strong>Nenhuma movimentação encontrada</strong>
          <p>Registre uma entrada ou saída para começar o histórico operacional.</p>
        </div>

        <div v-else class="movements-table-wrapper">
          <table class="movements-table">
            <thead>
              <tr>
                <th>Produto</th>
                <th>SKU</th>
                <th>Tipo</th>
                <th>Quantidade</th>
                <th>Motivo</th>
                <th>Observações</th>
                <th>Usuário</th>
                <th>Data</th>
              </tr>
            </thead>

            <tbody>
              <tr v-for="movement in paginatedMovements" :key="movement.id">
                <td>{{ movement.productName }}</td>
                <td>{{ movement.productSku }}</td>
                <td>
                  <span
                    class="status-badge"
                    :class="movement.type === 1 ? 'status-badge--success' : 'status-badge--danger'"
                  >
                    {{ movement.typeLabel }}
                  </span>
                </td>
                <td>{{ movement.quantity }}</td>
                <td>{{ movement.reason }}</td>
                <td class="movements-notes" :title="movement.notes || ''">
                  {{ movement.notes || '-' }}
                </td>
                <td>{{ movement.performedByUserName }}</td>
                <td>{{ formatDate(movement.createdAt) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div v-if="movements.length" class="pagination">
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

      <MovementModal
        v-if="showModal"
        :products="activeProducts"
        :loading="modalLoading"
        @close="closeModal"
        @submit="handleCreateMovement"
      />
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import AppLayout from '@/layouts/AppLayout.vue'
import MovementModal from '@/components/ui/MovementModal.vue'
import { getProductsRequest } from '@/api/products'
import { createStockMovementRequest, getStockMovementsRequest, exportStockMovementsCsvRequest } from '@/api/stockMovements'
import { useUiStore } from '@/stores/ui'
import { ArrowLeftRight, Plus, Download } from 'lucide-vue-next'
import { usePermissions } from '@/composables/usePermissions'

const loading = ref(false)
const modalLoading = ref(false)
const showModal = ref(false)

const products = ref<any[]>([])
const movements = ref<any[]>([])
const currentPage = ref(1)
const itemsPerPage = 6

const ui = useUiStore()
const { canManageMovements } = usePermissions()

const filters = reactive({
  productId: '',
  type: '',
  startDate: '',
  endDate: '',
})

const handleExportCsv = async () => {
  try {
    const blob = await exportStockMovementsCsvRequest({
      productId: filters.productId || undefined,
      type: filters.type === '' ? '' : Number(filters.type),
      startDate: filters.startDate || undefined,
      endDate: filters.endDate || undefined,
    })

    const url = window.URL.createObjectURL(new Blob([blob], { type: 'text/csv;charset=utf-8;' }))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', `movimentacoes_${new Date().toISOString().slice(0, 19).replace(/[:T]/g, '-')}.csv`)
    document.body.appendChild(link)
    link.click()
    link.remove()
    window.URL.revokeObjectURL(url)

    ui.showToast('success', 'CSV de movimentações exportado com sucesso.')
  } catch (error: any) {
    ui.showToast('error', 'Não foi possível exportar o CSV de movimentações.')
  }
}

const totalPages = computed(() => {
  return Math.max(1, Math.ceil(movements.value.length / itemsPerPage))
})

const paginatedMovements = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return movements.value.slice(start, end)
})

const activeProducts = computed(() => products.value.filter((item) => item.isActive))

const loadProducts = async () => {
  const response = await getProductsRequest()
  products.value = response
}

const loadMovements = async () => {
  loading.value = true

  try {
    const response = await getStockMovementsRequest({
      productId: filters.productId || undefined,
      type: filters.type === '' ? '' : Number(filters.type),
      startDate: filters.startDate || undefined,
      endDate: filters.endDate || undefined,
    })

    movements.value = response
    if (currentPage.value > totalPages.value) {
      currentPage.value = 1
    }
  } finally {
    loading.value = false
  }
}

watch(
  () => [filters.productId, filters.type, filters.startDate, filters.endDate],
  () => {
    currentPage.value = 1
  }
)

const handleCreateMovement = async (payload: any) => {
  modalLoading.value = true

  try {
    await createStockMovementRequest(payload)
    ui.showToast('success', 'Movimentação registrada com sucesso.')
    closeModal()
    await Promise.all([loadMovements(), loadProducts()])
  } catch (error: any) {
    ui.showToast('error', error?.response?.data?.message || 'Não foi possível registrar a movimentação.')
  } finally {
    modalLoading.value = false
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

const formatDate = (value: string) => {
  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'short',
    timeStyle: 'short',
  }).format(new Date(value))
}

onMounted(async () => {
  await Promise.all([loadProducts(), loadMovements()])
})
</script>

<style scoped>
.movements-page {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.movements-header,
.movements-filters,
.movements-table-section {
  padding: 24px;
}

.movements-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
}

.movements-header h2 {
  font-size: 24px;
  margin-bottom: 6px;
}

.movements-header p {
  color: var(--color-text-secondary);
}

.movements-filters {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
}

.movements-filters__grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 12px;
  width: 100%;
}

.movements-table-wrapper {
  overflow-x: auto;
}

.movements-table {
  width: 100%;
  border-collapse: collapse;
  border-spacing: 0;
}

.movements-table tbody tr {
  transition: transform 0.15s ease, background 0.2s ease;
}

.movements-table th,
.movements-table td {
  padding: 15px 12px;
  border-bottom: 1px solid var(--color-border);
  text-align: left;
  font-size: 14px;
}

.movements-table th {
  color: var(--color-text-secondary);
  font-weight: 700;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.movements-table tbody tr:hover {
  background: rgba(37, 99, 235, 0.03);
}

.movements-empty {
  color: var(--color-text-secondary);
}

.movements-filter-field {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.movements-filter-field label {
  font-size: 13px;
  font-weight: 700;
  color: var(--color-text-secondary);
}

.movements-notes {
  max-width: 240px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.movements-header__content {
  display: flex;
  align-items: center;
  gap: 16px;
}

.movements-header__icon {
  width: 50px;
  height: 50px;
  border-radius: 16px;
  background: rgba(14, 165, 233, 0.12);
  color: #0891b2;
  display: grid;
  place-items: center;
}

.movements-header__button {
  display: inline-flex;
  align-items: center;
  gap: 8px;
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

.movements-header__actions {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

@media (max-width: 1100px) {
  .movements-filters {
    flex-direction: column;
    align-items: stretch;
  }

  .movements-filters__grid {
    grid-template-columns: 1fr 1fr;
  }
}

@media (max-width: 640px) {
  .movements-header {
    flex-direction: column;
    align-items: stretch;
  }

  .movements-filters__grid {
    grid-template-columns: 1fr;
  }
}
</style>