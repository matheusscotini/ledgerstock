<template>
  <div class="modal-overlay" @click.self="$emit('close')">
    <div class="modal card">
      <div class="modal__header">
        <div>
          <h3>Nova movimentação</h3>
          <p>Registre entradas e saídas do estoque com rastreabilidade.</p>
        </div>

        <button class="modal__close" @click="$emit('close')">✕</button>
      </div>

      <form class="modal__form" @submit.prevent="handleSubmit">
        <div class="modal__grid">
          <div class="modal__field">
            <label>Produto</label>
            <select v-model="form.productId" class="input">
              <option value="">Selecione um produto</option>
              <option v-for="product in products" :key="product.id" :value="product.id">
                {{ product.name }} — {{ product.sku }}
              </option>
            </select>
          </div>

          <div class="modal__field">
            <label>Tipo</label>
            <select v-model.number="form.type" class="input">
              <option :value="1">Entrada</option>
              <option :value="2">Saída</option>
            </select>
          </div>

          <div class="modal__field">
            <label>Quantidade</label>
            <input v-model.number="form.quantity" class="input" type="number" min="1" step="1" />
          </div>

          <div class="modal__field">
            <label>Motivo</label>
            <input v-model="form.reason" class="input" type="text" />
          </div>
        </div>

        <div class="modal__field">
          <label>Observações</label>
          <textarea v-model="form.notes" class="modal__textarea" rows="4" />
        </div>

        <p v-if="errorMessage" class="modal__error">{{ errorMessage }}</p>

        <div class="modal__actions">
          <button type="button" class="btn btn-secondary" @click="$emit('close')">
            Cancelar
          </button>

          <button class="btn btn-primary" :disabled="loading">
            {{ loading ? 'Salvando...' : 'Salvar movimentação' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'

const props = defineProps<{
  products: Array<{
    id: string
    name: string
    sku: string
  }>
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', payload: any): void
}>()

const errorMessage = ref('')

const form = reactive({
  productId: '',
  type: 1,
  quantity: 1,
  reason: '',
  notes: '',
})

const handleSubmit = () => {
  errorMessage.value = ''

  if (!form.productId) {
    errorMessage.value = 'Selecione um produto.'
    return
  }

  if (!form.reason.trim()) {
    errorMessage.value = 'Informe o motivo da movimentação.'
    return
  }

  if (form.quantity <= 0) {
    errorMessage.value = 'A quantidade deve ser maior que zero.'
    return
  }

  emit('submit', {
    productId: form.productId,
    type: Number(form.type),
    quantity: Number(form.quantity),
    reason: form.reason,
    notes: form.notes,
  })
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  z-index: 50;
}

.modal {
  width: 100%;
  max-width: 760px;
  padding: 28px;
}

.modal__header {
  display: flex;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 24px;
}

.modal__header h3 {
  font-size: 24px;
  margin-bottom: 6px;
}

.modal__header p {
  color: var(--color-text-secondary);
}

.modal__close {
  width: 38px;
  height: 38px;
  border-radius: 10px;
  background: var(--color-surface-secondary);
}

.modal__form {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.modal__grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 16px;
}

.modal__field {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.modal__field label {
  font-size: 14px;
  font-weight: 600;
}

.modal__textarea {
  width: 100%;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: 14px;
  resize: vertical;
  outline: none;
}

.modal__textarea:focus {
  border-color: var(--color-primary);
  box-shadow: 0 0 0 4px rgba(37, 99, 235, 0.12);
}

.modal__actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.modal__error {
  color: var(--color-danger);
  font-size: 14px;
}

@media (max-width: 720px) {
  .modal__grid {
    grid-template-columns: 1fr;
  }
}
</style>