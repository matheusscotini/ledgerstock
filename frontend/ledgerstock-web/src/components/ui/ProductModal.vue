<template>
  <div class="modal-overlay" @click.self="$emit('close')">
    <div class="modal card">
      <div class="modal__header">
        <div>
          <h3>{{ isEditing ? 'Editar produto' : 'Novo produto' }}</h3>
          <p>Preencha os dados do produto para salvar no estoque.</p>
        </div>

        <button class="modal__close" @click="$emit('close')">✕</button>
      </div>

      <form class="modal__form" @submit.prevent="handleSubmit">
        <div class="modal__grid">
          <div class="modal__field">
            <label>Nome</label>
            <input v-model="form.name" class="input" type="text" />
          </div>

          <div class="modal__field">
            <label>SKU</label>
            <input v-model="form.sku" class="input" type="text" />
          </div>

          <div class="modal__field">
            <label>Categoria</label>
            <input v-model="form.category" class="input" type="text" />
          </div>

          <div class="modal__field">
            <label>Preço</label>
            <input v-model.number="form.price" class="input" type="number" min="0" step="0.01" />
          </div>

          <div class="modal__field">
            <label>Estoque mínimo</label>
            <input
              v-model.number="form.minimumStock"
              class="input"
              type="number"
              min="0"
              step="1"
            />
          </div>

          <div v-if="isEditing" class="modal__field modal__field--checkbox">
            <label>
              <input v-model="form.isActive" type="checkbox" />
              Produto ativo
            </label>
          </div>
        </div>

        <div class="modal__field">
          <label>Descrição</label>
          <textarea v-model="form.description" class="modal__textarea" rows="4" />
        </div>

        <p v-if="errorMessage" class="modal__error">{{ errorMessage }}</p>

        <div class="modal__actions">
          <button type="button" class="btn btn-secondary" @click="$emit('close')">
            Cancelar
          </button>

          <button class="btn btn-primary" :disabled="loading">
            {{ loading ? 'Salvando...' : isEditing ? 'Salvar alterações' : 'Cadastrar produto' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, reactive, watch, ref } from 'vue'

const props = defineProps<{
  modelValue?: any | null
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', payload: any): void
}>()

const errorMessage = ref('')

const form = reactive({
  name: '',
  sku: '',
  description: '',
  category: '',
  price: 0,
  minimumStock: 0,
  isActive: true,
})

const isEditing = computed(() => !!props.modelValue?.id)

watch(
  () => props.modelValue,
  (value) => {
    form.name = value?.name || ''
    form.sku = value?.sku || ''
    form.description = value?.description || ''
    form.category = value?.category || ''
    form.price = value?.price || 0
    form.minimumStock = value?.minimumStock || 0
    form.isActive = value?.isActive ?? true
    errorMessage.value = ''
  },
  { immediate: true }
)

const handleSubmit = () => {
  errorMessage.value = ''

  if (!form.name.trim() || !form.sku.trim()) {
    errorMessage.value = 'Nome e SKU são obrigatórios.'
    return
  }

  emit('submit', {
    name: form.name,
    sku: form.sku,
    description: form.description,
    category: form.category,
    price: Number(form.price),
    minimumStock: Number(form.minimumStock),
    isActive: form.isActive,
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
  animation: modalIn 0.22s ease;
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

.modal__field--checkbox {
  justify-content: flex-end;
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

@keyframes modalIn {
  from {
    opacity: 0;
    transform: translateY(14px) scale(0.98);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

@media (max-width: 720px) {
  .modal__grid {
    grid-template-columns: 1fr;
  }
}
</style>