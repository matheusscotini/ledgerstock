<template>
  <AuthLayout>
    <div class="auth-card card">
      <div class="auth-card__header">
        <h2>Entrar</h2>
        <p>Acesse sua conta para continuar.</p>
      </div>

      <form class="auth-card__form" @submit.prevent="handleSubmit">
        <div class="auth-card__field">
          <label>E-mail</label>
          <input v-model="form.email" type="email" class="input" placeholder="seu@email.com" />
        </div>

        <div class="auth-card__field">
          <label>Senha</label>
          <input v-model="form.password" type="password" class="input" placeholder="Sua senha" />
        </div>

        <p v-if="errorMessage" class="auth-card__error">{{ errorMessage }}</p>

        <button class="btn btn-primary auth-card__submit" :disabled="auth.loading">
          {{ auth.loading ? 'Entrando...' : 'Entrar' }}
        </button>

        <RouterLink to="/register" class="auth-card__link">
          Ainda não tem conta? Cadastre-se
        </RouterLink>
      </form>
    </div>
  </AuthLayout>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import AuthLayout from '@/layouts/AuthLayout.vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()

const errorMessage = ref('')

const form = reactive({
  email: '',
  password: '',
})

const handleSubmit = async () => {
  errorMessage.value = ''

  try {
    await auth.login({
      email: form.email,
      password: form.password,
    })

    router.push('/dashboard')
  } catch (error: any) {
    errorMessage.value =
      error?.response?.data?.message || 'Não foi possível realizar o login.'
  }
}
</script>

<style scoped>
.auth-card {
  width: 100%;
  max-width: 460px;
  padding: 36px;
  box-shadow: var(--shadow-md);
}

.auth-card__header {
  margin-bottom: 24px;
}

.auth-card__header h2 {
  font-size: 30px;
  margin-bottom: 8px;
  letter-spacing: -0.02em;
}

.auth-card__header p {
  color: var(--color-text-secondary);
  line-height: 1.6;
}

.auth-card__form {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.auth-card__field {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.auth-card__field label {
  font-size: 14px;
  font-weight: 700;
}

.auth-card__error {
  font-size: 14px;
  color: var(--color-danger);
}

.auth-card__submit {
  width: 100%;
}

.auth-card__link {
  font-size: 14px;
  color: var(--color-primary);
  text-align: center;
  font-weight: 700;
}
</style>