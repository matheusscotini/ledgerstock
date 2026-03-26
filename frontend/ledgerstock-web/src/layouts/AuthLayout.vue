<template>
  <div class="auth-layout">
    <div class="auth-layout__left">
      <div class="auth-layout__content">
        <span class="auth-layout__badge">LedgerStock</span>
        <h1>Gestão de estoque com visão profissional</h1>
        <p>
          Controle produtos, movimentações e acompanhe indicadores do estoque
          com uma experiência clara, segura e organizada.
        </p>
      </div>
    </div>

    <div class="auth-layout__right">
      <slot />
    </div>
  </div>
</template>

<style scoped>
.auth-layout {
  min-height: 100vh;
  display: grid;
  grid-template-columns: 1.1fr 0.9fr;
}

.auth-layout__left {
  background: linear-gradient(135deg, #0f172a, #1e3a8a);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 48px;
}

.auth-layout__content {
  max-width: 480px;
  animation: fadeUp 0.6s ease;
}

.auth-layout__badge {
  display: inline-flex;
  padding: 8px 14px;
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.12);
  font-size: 14px;
  margin-bottom: 20px;
}

.auth-layout__content h1 {
  font-size: 42px;
  line-height: 1.1;
  margin-bottom: 18px;
}

.auth-layout__content p {
  font-size: 16px;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.84);
}

.auth-layout__right {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 32px;
  background:
    radial-gradient(circle at top right, rgba(37, 99, 235, 0.08), transparent 28%),
    #f8fafc;
}

@keyframes authCardIn {
  from {
    opacity: 0;
    transform: translateY(14px) scale(0.98);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

@media (max-width: 960px) {
  .auth-layout {
    grid-template-columns: 1fr;
  }

  .auth-layout__left {
    display: none;
  }
}
</style>