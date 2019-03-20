<template>
  <div class="layout">
    <app-nav />
    <div class="container">
      <nuxt />
    </div>
    <app-footer />
  </div>
</template>

<script>
import AppNav from '~/components/AppNav.vue'
import AppFooter from '~/components/AppFooter.vue'

export default {
  components: {
    AppNav,
    AppFooter
  }
}
</script>

<style lang="scss" scoped>
.container {
  padding-top: 10px;
  padding-bottom: 10px;
  min-height: calc(100vh - 186px);
}
</style>
