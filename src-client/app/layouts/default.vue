<template>
  <div>
    <app-nav />
    <div class="container-fluid">
      <nuxt />
    </div>
  </div>
</template>

<script>
import AppNav from '~/components/Nav.vue'

export default {
  components: {
    AppNav
  }
}
</script>

<style></style>
