<template>
  <div class="toast-wrapper">
    <div
      v-for="toast in ui.toasts"
      :key="toast.id"
      class="toast-item"
      :class="`toast-item--${toast.type}`"
    >
      {{ toast.message }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { useUiStore } from '@/stores/ui'

const ui = useUiStore()
</script>

<style scoped>
.toast-wrapper {
  position: fixed;
  top: 22px;
  right: 22px;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.toast-item {
  min-width: 280px;
  max-width: 360px;
  padding: 14px 16px;
  border-radius: 16px;
  color: white;
  font-weight: 700;
  box-shadow: var(--shadow-lg);
}

.toast-item--success {
  background: linear-gradient(135deg, #16a34a, #15803d);
}

.toast-item--error {
  background: linear-gradient(135deg, #dc2626, #b91c1c);
}

.toast-item--info {
  background: linear-gradient(135deg, #2563eb, #1d4ed8);
}
</style>