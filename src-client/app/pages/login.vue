<template>
  <div class="page">
    <div class="centered">
      <h2>Login</h2>
      <b-form v-if="show" @submit="onSubmit" @reset="onReset">
        <b-form-group
          id="userNameInputGroup"
          label="Username:"
          label-for="userNameInput"
          description="Your username."
        >
          <b-form-input
            id="userNameInput"
            v-model="form.username"
            type="text"
            required
            placeholder="Enter username"
          />
        </b-form-group>

        <b-form-group
          id="passwordInputGroup"
          label="Password:"
          label-for="passwordInput"
          description="Your password."
        >
          <b-form-input
            id="passwordInput"
            v-model="form.password"
            type="password"
            required
            placeholder="Enter password"
          />
        </b-form-group>

        <b-form-group id="rememberMeGroup">
          <b-form-checkbox id="rememberMeCheckbox" v-model="form.rememberMe"
            >Remember me</b-form-checkbox
          >
        </b-form-group>

        <b-alert v-model="hasErrorMessage" variant="danger" dismissible>
          {{ errorMessage }}
        </b-alert>

        <b-button type="submit" variant="primary">Submit</b-button>
        <b-button type="reset" variant="link">Reset</b-button> |
        <b-button type="button" variant="link" to="/register"
          >Register</b-button
        >
      </b-form>
    </div>
  </div>
</template>

<script>
import { setToken } from '~/assets/js/auth'
import { getQueryParam } from '~/assets/js/urls'

export default {
  middleware: 'notAuthenticated',
  data() {
    return {
      form: {
        username: '',
        password: '',
        rememberMe: false
      },
      errorMessage: null,
      show: true
    }
  },
  computed: {
    hasErrorMessage() {
      return this.errorMessage && this.errorMessage.length > 0
    }
  },
  methods: {
    async onSubmit(evt) {
      evt.preventDefault()
      this.errorMessage = null
      try {
        const response = await this.$axios.post(
          '/api/auth/authenticate',
          this.form
        )
        setToken(response.data.token, this.form.rememberMe)
        const { id, firstName, lastName, username } = response.data
        this.$store.commit('setUser', { id, firstName, lastName, username })
        const redirect = getQueryParam('redirect')
        this.$router.push(redirect && redirect.length > 0 ? redirect : '/')
        // alert(JSON.stringify(response.data))
      } catch (err) {
        if (err.response && err.response.data && err.response.data.message) {
          this.errorMessage = err.response.data.message
        } else {
          this.errorMessage = JSON.stringify(err.response)
        }
      }
    },
    onReset(evt) {
      evt.preventDefault()
      this.errorMessage = null
      this.form.username = ''
      this.form.password = ''
      this.form.rememberMe = false
      /* Trick to reset/clear native browser form validation state */
      this.show = false
      this.$nextTick(() => {
        this.show = true
      })
    }
  }
}
</script>

<style lang="scss" scoped>
.page {
  min-height: 550px;
  .centered {
    @include centerTop;
    top: 150px;
    min-width: 350px;
  }
}
</style>
