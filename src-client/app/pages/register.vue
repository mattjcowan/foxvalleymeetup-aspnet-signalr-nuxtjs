<template>
  <div class="centered">
    <h2>Register</h2>
    <b-form v-if="show" @submit="onSubmit" @reset="onReset">
      <b-form-group
        id="firstNameInputGroup"
        label="First name:"
        label-for="firstNameInput"
        description="Your first name."
      >
        <b-form-input
          id="firstNameInput"
          v-model="form.firstname"
          type="text"
          required
          placeholder="Enter first name"
        />
      </b-form-group>

      <b-form-group
        id="lastNameInputGroup"
        label="Last name:"
        label-for="lastNameInput"
        description="Your last name."
      >
        <b-form-input
          id="lastNameInput"
          v-model="form.lastname"
          type="text"
          required
          placeholder="Enter last name"
        />
      </b-form-group>

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

      <b-alert v-model="hasErrorMessage" variant="danger" dismissible>
        {{ errorMessage }}
      </b-alert>

      <b-button type="submit" variant="primary">Submit</b-button>
      <b-button type="reset" variant="link">Reset</b-button> |
      <b-button type="button" variant="link" to="/login">Login</b-button>
    </b-form>
  </div>
</template>

<script>
export default {
  data() {
    return {
      form: {
        firstname: '',
        lastname: '',
        username: '',
        password: ''
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
        await this.$axios.post('/api/auth/register', this.form)
        this.$router.push('/login')
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
      this.form.firstname = ''
      this.form.lastname = ''
      this.form.username = ''
      this.form.password = ''
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
