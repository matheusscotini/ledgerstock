<template>
  <AppLayout>
    <div class="products-page">
      <div class="products-header card">
        <div class="products-header__content">
          <div class="products-header__icon">
            <Package :size="22" />
          </div>

          <div>
            <h2>Produtos</h2>
            <p>Cadastre, edite e acompanhe o status do estoque de cada item.</p>
          </div>
        </div>

        <button
          v-if="canManageProducts"
          class="btn btn-primary products-header__button"
          @click="openCreateModal"
        >
          <Plus :size="16" />
          <span>Novo produto</span>
        </button>
      </div>

      <div class="products-filters card">
        <div class="products-filters__group">
          <input
            v-model="filters.search"
            class="input"
            type="text"
            placeholder="Buscar por nome, SKU ou categoria"
          />

          <select v-model="filters.isActive" class="input">
            <option value="">Todos os status</option>
            <option value="true">Ativos</option>
            <option value="false">Inativos</option>
          </select>
        </div>

        <button class="btn btn-secondary" @click="loadProducts">Filtrar</button>
      </div>

      <div class="products-table-section card">
        <div v-if="loading" class="products-empty">Carregando produtos...</div>

        <div v-else-if="!products.length" class="empty-state">
          <strong>Nenhum produto encontrado</strong>
          <p>Tente ajustar os filtros ou cadastre um novo item no estoque.</p>  
        </div>

        <div v-else class="products-table-wrapper">
          <table class="products-table">
            <thead>
              <tr>
                <th>Produto</th>
                <th>SKU</th>
                <th>Categoria</th>
                <th>Preço</th>
                <th>Estoque</th>
                <th>Status do estoque</th>
                <th>Situação</th>
                <th v-if="canManageProducts">Ações</th>
              </tr>
            </thead>

            <tbody>
              <tr v-for="product in paginatedProducts" :key="product.id">
                <td>{{ product.name }}</td>
                <td>{{ product.sku }}</td>
                <td>{{ product.category || '-' }}</td>
                <td>{{ formatCurrency(product.price) }}</td>
                <td>{{ product.currentStock }}</td>
                <td>
                  <span
                    class="status-badge"
                    :class="getStockBadgeClass(product.stockStatus)"
                  >
                    {{ product.stockStatus }}
                  </span>
                </td>
                <td>
                  <span
                    class="status-badge"
                    :class="product.isActive ? 'status-badge--success' : 'status-badge--inactive'"
                  >
                    {{ product.isActive ? 'Ativo' : 'Inativo' }}
                  </span>
                </td>
                <td v-if="canManageProducts">
                  <div class="products-actions">
                    <button class="products-link" @click="openEditModal(product)">
                      Editar
                    </button>

                    <button class="products-link" @click="toggleStatus(product.id)">
                      {{ product.isActive ? 'Inativar' : 'Ativar' }}
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div v-if="products.length" class="pagination">
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

      <ProductModal
        v-if="showModal"
        :model-value="selectedProduct"
        :loading="modalLoading"
        @close="closeModal"
        @submit="handleSubmit"
      />
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import AppLayout from '@/layouts/AppLayout.vue'
import ProductModal from '@/components/ui/ProductModal.vue'
import { Package, Plus, Search } from 'lucide-vue-next'
import { usePermissions } from '@/composables/usePermissions'
import { useAuthStore } from '@/stores/auth'
import { watchEffect } from 'vue'
import { useUiStore } from '@/stores/ui'
import {
  createProductRequest,
  getProductsRequest,
  toggleProductStatusRequest,
  updateProductRequest,
} from '@/api/products'

const loading = ref(false)
const modalLoading = ref(false)
const showModal = ref(false)
const selectedProduct = ref<any | null>(null)
const products = ref<any[]>([])
const currentPage = ref(1)
const itemsPerPage = 6
const ui = useUiStore()

const { roles, canManageProducts } = usePermissions()

const auth = useAuthStore()

watchEffect(() => {
  console.log('USER', auth.user)
  console.log('ROLES', roles.value)
  console.log('canManageProducts', canManageProducts.value)
})

const filters = reactive({
  search: '',
  isActive: '',
})

const totalPages = computed(() => {
  return Math.max(1, Math.ceil(products.value.length / itemsPerPage))
})

const paginatedProducts = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return products.value.slice(start, end)
})

const loadProducts = async () => {
  loading.value = true

  try {
    const response = await getProductsRequest({
      search: filters.search || undefined,
      isActive: filters.isActive === '' ? '' : filters.isActive === 'true',
    })

    products.value = response
    if (currentPage.value > totalPages.value) {
      currentPage.value = 1
    }
  } finally {
    loading.value = false
  }
}
watch(
  () => [filters.search, filters.isActive],
  () => {
    currentPage.value = 1
  }
)

const openCreateModal = () => {
  selectedProduct.value = null
  showModal.value = true
}

const openEditModal = (product: any) => {
  selectedProduct.value = { ...product }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
  selectedProduct.value = null
}

const handleSubmit = async (payload: any) => {
  modalLoading.value = true

  try {
    if (selectedProduct.value?.id) {
      await updateProductRequest(selectedProduct.value.id, payload)
      ui.showToast('success', 'Produto atualizado com sucesso.')
    } else {
      await createProductRequest(payload)
      ui.showToast('success', 'Produto cadastrado com sucesso.')
    }

    closeModal()
    await loadProducts()
  } catch (error: any) {
    ui.showToast('error', error?.response?.data?.message || 'Não foi possível salvar o produto.')
  } finally {
    modalLoading.value = false
  }
}

const toggleStatus = async (id: string) => {
  try {
    await toggleProductStatusRequest(id)
    ui.showToast('success', 'Status do produto alterado com sucesso.')
    await loadProducts()
  } catch (error: any) {
    ui.showToast('error', error?.response?.data?.message || 'Não foi possível alterar o status.')
  }
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

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  }).format(value)
}

const getStockBadgeClass = (status: string) => {
  if (status === 'Sem estoque') return 'status-badge--danger'
  if (status === 'Estoque baixo') return 'status-badge--warning'
  return 'status-badge--success'
}

onMounted(() => {
  loadProducts()
})
</script>

<style scoped>
.products-page {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.products-header,
.products-filters,
.products-table-section {
  padding: 24px;
}

.products-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
}

.products-header h2 {
  font-size: 24px;
  margin-bottom: 6px;
}

.products-header p {
  color: var(--color-text-secondary);
}

.products-filters {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
}

.products-filters__group {
  display: grid;
  grid-template-columns: 1.5fr 240px;
  gap: 12px;
  width: 100%;
  max-width: 760px;
}

.products-table-wrapper {
  overflow-x: auto;
}

.products-table {
  width: 100%;
  border-collapse: collapse;
  border-spacing: 0;
}

.products-table tbody tr {
  transition: transform 0.15s ease, background 0.2s ease;
}

.products-table th,
.products-table td {
  padding: 15px 12px;
  border-bottom: 1px solid var(--color-border);
  text-align: left;
  font-size: 14px;
}

.products-table th {
  color: var(--color-text-secondary);
  font-weight: 700;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.products-table tbody tr {
  transition: background 0.2s ease;
}

.products-table tbody tr:hover {
  background: rgba(37, 99, 235, 0.03);
}

.products-actions {
  display: flex;
  gap: 12px;
}

.products-link {
  color: var(--color-primary);
  font-weight: 700;
}

.products-empty {
  color: var(--color-text-secondary);
}

.products-header__content {
  display: flex;
  align-items: center;
  gap: 16px;
}

.products-header__icon {
  width: 50px;
  height: 50px;
  border-radius: 16px;
  background: rgba(37, 99, 235, 0.1);
  color: #2563eb;
  display: grid;
  place-items: center;
}

.products-header__button {
  display: inline-flex;
  align-items: center;
  gap: 8px;
}

.products-search {
  position: relative;
  display: flex;
  align-items: center;
}

.products-search svg {
  position: absolute;
  left: 14px;
  color: var(--color-text-muted);
  pointer-events: none;
}

.products-search__input {
  padding-left: 40px;
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

@media (max-width: 960px) {
  .products-header,
  .products-filters {
    flex-direction: column;
    align-items: stretch;
  }

  .products-filters__group {
    grid-template-columns: 1fr;
    max-width: none;
  }
}
</style>