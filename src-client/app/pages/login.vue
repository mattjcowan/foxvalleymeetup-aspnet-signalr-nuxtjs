<template>
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
      <b-button type="button" variant="link" to="/register">Register</b-button>
    </b-form>
  </div>
</template>

<script>
export default {
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
        alert(JSON.stringify(response.data))
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
      /* Reset our form values */
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
.centered {
  position: absolute;
  top: 150px;
  left: 50%;
  transform: translate3d(-50%, 0, 0);
  // @include center;
  // top: 300px;
  min-width: 350px;
}
</style>
