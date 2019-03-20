<template>
  <div></div>
</template>

<script>
import { unsetToken } from '~/assets/js/auth'

export default {
  middleware: 'authenticated',
  mounted() {
    unsetToken()
    this.$store.commit('setUser', null)
    this.$router.push('/')
  }
}
</script>
