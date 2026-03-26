<template>
  <div class="modal-overlay" @click.self="$emit('close')">
    <div class="modal card">
      <div class="modal__header">
        <div>
          <h3>Novo usuário</h3>
          <p>Crie um usuário e defina o perfil de acesso ao sistema.</p>
        </div>

        <button class="modal__close" @click="$emit('close')">✕</button>
      </div>

      <form class="modal__form" @submit.prevent="handleSubmit">
        <div class="modal__grid">
          <div class="modal__field">
            <label>Nome completo</label>
            <input v-model="form.fullName" class="input" type="text" />
          </div>

          <div class="modal__field">
            <label>E-mail</label>
            <input v-model="form.email" class="input" type="email" />
          </div>

          <div class="modal__field">
            <label>Senha</label>
            <input v-model="form.password" class="input" type="password" />
          </div>

          <div class="modal__field">
            <label>Perfil</label>
            <select v-model="form.role" class="input">
              <option value="Standard">Padrão</option>
              <option value="Admin">Admin</option>
              <option value="Master">Master</option>
            </select>
          </div>
        </div>

        <p v-if="errorMessage" class="modal__error">{{ errorMessage }}</p>

        <div class="modal__actions">
          <button type="button" class="btn btn-secondary" @click="$emit('close')">
            Cancelar
          </button>

          <button class="btn btn-primary" :disabled="loading">
            {{ loading ? 'Salvando...' : 'Criar usuário' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'

const props = defineProps<{
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', payload: any): void
}>()

const errorMessage = ref('')

const form = reactive({
  fullName: '',
  email: '',
  password: '',
  role: 'Standard',
})

const handleSubmit = () => {
  errorMessage.value = ''

  if (!form.fullName.trim() || !form.email.trim() || !form.password.trim()) {
    errorMessage.value = 'Preencha nome, e-mail e senha.'
    return
  }

  emit('submit', {
    fullName: form.fullName,
    email: form.email,
    password: form.password,
    role: form.role,
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
  max-width: 720px;
  padding: 28px;
  animation: modalIn 0.22s ease;
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
  font-weight: 700;
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