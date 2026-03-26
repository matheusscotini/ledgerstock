<template>
  <div class="app-layout">
    <aside class="app-layout__sidebar">
      <div>
        <div class="app-layout__brand">
          <div class="app-layout__brand-icon">L</div>
          <div>
            <strong>LedgerStock</strong>
            <span>Inventory System - LABEST</span>
          </div>
        </div>

        <nav class="app-layout__nav">
          <RouterLink to="/dashboard">Dashboard</RouterLink>
          <RouterLink to="/products">Produtos</RouterLink>
          <RouterLink to="/movements">Movimentações</RouterLink>
          <RouterLink v-if="isMaster" to="/users">Usuários</RouterLink>
        </nav>
      </div>

      <div class="app-layout__footer">
        <div class="app-layout__user">
          <div class="app-layout__avatar">
            {{ auth.user?.fullName?.charAt(0)?.toUpperCase() || 'U' }}
          </div>

          <div>
            <strong>{{ auth.user?.fullName || 'Usuário' }}</strong>
            <span>{{ auth.user?.email || 'Sem e-mail' }}</span>
          </div>
        </div>

        <button class="app-layout__logout" @click="handleLogout">Sair</button>
      </div>
    </aside>

    <main class="app-layout__content">
      <header class="app-layout__header">
        <div>
          <h1>Controle operacional do estoque</h1>
          <p>
            Acompanhe produtos, movimentações e indicadores em uma visão centralizada.
          </p>
        </div>
      </header>

      <section class="app-layout__body">
        <slot />
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { usePermissions } from '@/composables/usePermissions'

const auth = useAuthStore()
const router = useRouter()
const { isMaster } = usePermissions()

const handleLogout = () => {
  auth.logout()
  router.push('/login')
}
</script>

<style scoped>
.app-layout {
  min-height: 100vh;
  display: grid;
  grid-template-columns: 280px 1fr;
  background:
    radial-gradient(circle at top left, rgba(37, 99, 235, 0.06), transparent 28%),
    var(--color-background);
}

.app-layout__sidebar {
  background: linear-gradient(180deg, #0f172a 0%, #111827 100%);
  color: white;
  padding: 24px 18px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  border-right: 1px solid rgba(255, 255, 255, 0.04);
  position: sticky;
  top: 0;
  height: 100vh;
}

.app-layout__brand {
  display: flex;
  align-items: center;
  gap: 14px;
  margin-bottom: 34px;
}

.app-layout__brand-icon {
  width: 46px;
  height: 46px;
  border-radius: 14px;
  display: grid;
  place-items: center;
  background: linear-gradient(135deg, #2563eb, #1d4ed8);
  font-weight: 800;
  font-size: 20px;
}

.app-layout__brand strong {
  display: block;
  font-size: 18px;
}

.app-layout__brand span {
  display: block;
  font-size: 12px;
  color: rgba(255, 255, 255, 0.65);
  margin-top: 4px;
}

.app-layout__nav {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.app-layout__nav a {
  padding: 13px 14px;
  border-radius: 14px;
  color: rgba(255, 255, 255, 0.78);
  transition: 0.2s ease;
  font-weight: 600;
}

.app-layout__nav a:hover {
  background: rgba(255, 255, 255, 0.06);
  color: white;
}

.app-layout__nav a.router-link-active {
  background: linear-gradient(135deg, rgba(37, 99, 235, 0.28), rgba(59, 130, 246, 0.18));
  color: white;
  box-shadow: inset 0 0 0 1px rgba(96, 165, 250, 0.18);
}

.app-layout__footer {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.app-layout__user {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 14px;
  border-radius: 16px;
  background: rgba(255, 255, 255, 0.05);
}

.app-layout__avatar {
  width: 42px;
  height: 42px;
  border-radius: 50%;
  display: grid;
  place-items: center;
  background: rgba(255, 255, 255, 0.12);
  font-weight: 800;
}

.app-layout__user strong {
  display: block;
  font-size: 14px;
}

.app-layout__user span {
  display: block;
  font-size: 12px;
  color: rgba(255, 255, 255, 0.64);
  margin-top: 4px;
}

.app-layout__logout {
  height: 44px;
  border-radius: 14px;
  background: rgba(255, 255, 255, 0.08);
  color: white;
  font-weight: 700;
  transition: 0.2s ease;
}

.app-layout__logout:hover {
  background: rgba(255, 255, 255, 0.14);
}

.app-layout__content {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.app-layout__header {
  padding: 28px 32px 8px;
}

.app-layout__header h1 {
  font-size: 30px;
  font-weight: 800;
  margin-bottom: 6px;
  letter-spacing: -0.02em;
}

.app-layout__header p {
  color: var(--color-text-secondary);
  max-width: 720px;
  line-height: 1.6;
}

.app-layout__body {
  padding: 24px 32px 32px;
}

@media (max-width: 980px) {
  .app-layout {
    grid-template-columns: 1fr;
  }

  .app-layout__sidebar {
    gap: 20px;
  }
}
</style>