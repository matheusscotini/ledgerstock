<template>
  <AppLayout>
    <div class="dashboard-page">
      <section class="dashboard-grid">
        <div class="dashboard-card card">
          <div class="dashboard-card__icon dashboard-card__icon--blue">
            <Boxes :size="20" />
          </div>
          <span class="dashboard-card__label">Total de produtos</span>
          <strong>{{ summary.totalProducts }}</strong>
        </div>

        <div class="dashboard-card card">
          <div class="dashboard-card__icon dashboard-card__icon--green">
            <BadgeCheck :size="20" />
          </div>
          <span class="dashboard-card__label">Produtos ativos</span>
          <strong>{{ summary.activeProducts }}</strong>
        </div>

        <div class="dashboard-card card">
          <div class="dashboard-card__icon dashboard-card__icon--cyan">
            <ArrowLeftRight :size="20" />
          </div>
          <span class="dashboard-card__label">Movimentações</span>
          <strong>{{ summary.totalMovements }}</strong>
        </div>

        <div class="dashboard-card card">
          <div class="dashboard-card__icon dashboard-card__icon--red">
            <AlertTriangle :size="20" />
          </div>
          <span class="dashboard-card__label">Produtos sem estoque</span>
          <strong>{{ summary.productsOutOfStock }}</strong>
        </div>
      </section>

      <section class="dashboard-section card">
        <div class="dashboard-section__header">
          <div>
            <h2>Visão geral do estoque</h2>
            <p>Resumo operacional com base nas movimentações registradas.</p>
          </div>
        </div>

        <div v-if="loading" class="dashboard-skeleton-grid">
          <div class="skeleton-card" v-for="item in 4" :key="item"></div>
        </div>

        <div v-else class="dashboard-stats">
          <div class="dashboard-stat-box">
            <span>Entradas</span>
            <strong>{{ summary.totalEntries }}</strong>
          </div>

          <div class="dashboard-stat-box">
            <span>Saídas</span>
            <strong>{{ summary.totalExits }}</strong>
          </div>

          <div class="dashboard-stat-box">
            <span>Estoque baixo</span>
            <strong>{{ summary.productsWithLowStock }}</strong>
          </div>

          <div class="dashboard-stat-box">
            <span>Inativos</span>
            <strong>{{ summary.inactiveProducts }}</strong>
          </div>
        </div>
      </section>

      <section class="dashboard-section card">
        <div class="dashboard-section__header">
          <div>
            <h2>Produtos com atenção</h2>
            <p>Itens com estoque baixo ou zerado.</p>
          </div>
        </div>

        <div v-if="loading" class="dashboard-table-skeleton">
          <div class="skeleton-row" v-for="item in 4" :key="item"></div>
        </div>

        <div v-else-if="!lowStockProducts.length" class="empty-state">
          <strong>Sem produtos em alerta</strong>
          <p>Todos os itens ativos estão com estoque acima do mínimo definido.</p>
        </div>

        <div v-else class="dashboard-table-wrapper">
          <table class="dashboard-table">
            <thead>
              <tr>
                <th>Produto</th>
                <th>SKU</th>
                <th>Categoria</th>
                <th>Estoque atual</th>
                <th>Estoque mínimo</th>
                <th>Status</th>
              </tr>
            </thead>

            <tbody>
              <tr v-for="item in paginatedLowStockProducts" :key="item.id">
                <td>{{ item.name }}</td>
                <td>{{ item.sku }}</td>
                <td>{{ item.category || '-' }}</td>
                <td>{{ item.currentStock }}</td>
                <td>{{ item.minimumStock }}</td>
                <td>
                  <span
                    class="status-badge"
                    :class="item.isOutOfStock ? 'status-badge--danger' : 'status-badge--warning'"
                  >
                    {{ item.stockStatus }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
      <div v-if="lowStockProducts.length" class="pagination">
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
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import AppLayout from '@/layouts/AppLayout.vue'
import { getDashboardSummaryRequest, getLowStockProductsRequest } from '@/api/dashboard'
import { Boxes, BadgeCheck, ArrowLeftRight, AlertTriangle } from 'lucide-vue-next'

const loading = ref(false)

const summary = reactive({
  totalProducts: 0,
  activeProducts: 0,
  inactiveProducts: 0,
  totalMovements: 0,
  totalEntries: 0,
  totalExits: 0,
  productsWithLowStock: 0,
  productsOutOfStock: 0,
})

const lowStockProducts = ref<any[]>([])
const currentPage = ref(1)
const itemsPerPage = 6

const totalPages = computed(() => {
  return Math.max(1, Math.ceil(lowStockProducts.value.length / itemsPerPage))
})

const paginatedLowStockProducts = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return lowStockProducts.value.slice(start, end)
})

const loadDashboard = async () => {
  loading.value = true

  try {
    const [summaryResponse, lowStockResponse] = await Promise.all([
      getDashboardSummaryRequest(),
      getLowStockProductsRequest(),
    ])

    Object.assign(summary, summaryResponse)
    lowStockProducts.value = lowStockResponse
    if (currentPage.value > totalPages.value) {
      currentPage.value = 1
    }
  } finally {
    loading.value = false
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

onMounted(() => {
  loadDashboard()
})
</script>

<style scoped>
.dashboard-page {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 18px;
}

.dashboard-card {
  padding: 24px;
  position: relative;
  overflow: hidden;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.dashboard-card:hover {
  transform: translateY(-3px);
  box-shadow: var(--shadow-md);
}

.dashboard-card__icon {
  width: 42px;
  height: 42px;
  border-radius: 14px;
  display: grid;
  place-items: center;
  margin-bottom: 16px;
}

.dashboard-card__icon--blue {
  background: rgba(37, 99, 235, 0.12);
  color: #2563eb;
}

.dashboard-card__icon--green {
  background: rgba(22, 163, 74, 0.12);
  color: #15803d;
}

.dashboard-card__icon--cyan {
  background: rgba(14, 165, 233, 0.12);
  color: #0891b2;
}

.dashboard-card__icon--red {
  background: rgba(220, 38, 38, 0.12);
  color: #dc2626;
}

.dashboard-card__label {
  display: block;
  font-size: 13px;
  color: var(--color-text-secondary);
  margin-bottom: 12px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}

.dashboard-card strong {
  font-size: 34px;
  letter-spacing: -0.02em;
}

.dashboard-section {
  padding: 26px;
}

.dashboard-section__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 22px;
}

.dashboard-section__header h2 {
  font-size: 21px;
  margin-bottom: 6px;
}

.dashboard-section__header p {
  color: var(--color-text-secondary);
}

.dashboard-stats {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
}

.dashboard-stat-box {
  background: linear-gradient(180deg, #f8fafc 0%, #ffffff 100%);
  border: 1px solid var(--color-border);
  border-radius: 18px;
  padding: 20px;
  transition: transform 0.2s ease;
}

.dashboard-stat-box:hover {
  transform: translateY(-2px);
}

.dashboard-stat-box span {
  display: block;
  color: var(--color-text-secondary);
  margin-bottom: 8px;
  font-size: 13px;
  font-weight: 700;
  text-transform: uppercase;
}

.dashboard-stat-box strong {
  font-size: 28px;
}

.dashboard-table-wrapper {
  overflow-x: auto;
}

.dashboard-table {
  width: 100%;
  border-collapse: collapse;
  border-spacing: 0;
}

.dashboard-table tbody tr {
  transition: transform 0.15s ease, background 0.2s ease;
}

.dashboard-table th,
.dashboard-table td {
  text-align: left;
  padding: 15px 12px;
  border-bottom: 1px solid var(--color-border);
  font-size: 14px;
}

.dashboard-table tbody tr:hover {
  background: rgba(37, 99, 235, 0.03);
}

.dashboard-table th {
  color: var(--color-text-secondary);
  font-weight: 700;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.dashboard-skeleton-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
}

.skeleton-card,
.skeleton-row {
  position: relative;
  overflow: hidden;
  background: #e2e8f0;
}

.skeleton-card {
  height: 110px;
  border-radius: 18px;
}

.dashboard-table-skeleton {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.skeleton-row {
  height: 54px;
  border-radius: 14px;
}

.skeleton-card::after,
.skeleton-row::after {
  content: '';
  position: absolute;
  inset: 0;
  transform: translateX(-100%);
  background: linear-gradient(
    90deg,
    transparent,
    rgba(255,255,255,0.5),
    transparent
  );
  animation: shimmer 1.3s infinite;
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

@keyframes shimmer {
  100% {
    transform: translateX(100%);
  }
}

@media (max-width: 1100px) {
  .dashboard-grid,
  .dashboard-stats,
  .dashboard-skeleton-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 700px) {
  .dashboard-grid,
  .dashboard-stats,
  .dashboard-skeleton-grid {
    grid-template-columns: 1fr;
  }
}
</style>