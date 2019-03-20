<template>
  <b-navbar
    toggleable="lg"
    type="dark"
    variant="primary"
    class="app-navbar"
    sticky
  >
    <div class="container">
      <b-navbar-brand to="/" exact>
        <img class="d-block" height="65" src="/logo.svg" />
        <span class="sr-only">Home</span>
      </b-navbar-brand>

      <b-navbar-toggle target="nav_collapse" />

      <b-collapse id="nav_collapse" is-nav>
        <b-navbar-nav>
          <b-nav-item
            v-for="item in nav"
            :key="item.title"
            :to="item.to"
            :href="item.href"
            :exact="item.exact"
            active-class="active"
            ><i v-if="item.iclass" :class="item.iclass" aria-hidden="true"></i>
            {{ item.title }}</b-nav-item
          >
        </b-navbar-nav>

        <!-- Right aligned nav items -->
        <b-navbar-nav class="ml-auto">
          <b-nav-item
            v-if="!isAuthenticated"
            to="/login"
            exact
            active-class="active"
            >Login</b-nav-item
          >
          <b-nav-item-dropdown v-if="isAuthenticated" right>
            <!-- Using button-content slot -->
            <template slot="button-content"
              ><em>{{ user.firstName + ' ' + user.lastName }}</em></template
            >
            <b-dropdown-item href="#" disabled>Profile</b-dropdown-item>
            <b-dropdown-item to="/logout">Logout</b-dropdown-item>
          </b-nav-item-dropdown>

          <b-nav-item
            href="https://github.com/mattjcowan/foxvalleymeetup-aspnet-signalr-nuxtjs"
            active-class="active"
            ><i class="fa fa-github" aria-hidden="true"></i
          ></b-nav-item>

          <b-nav-item
            href="https://twitter.com/mattjcowan"
            active-class="active"
            ><i class="fa fa-twitter" aria-hidden="true"></i
          ></b-nav-item>

          <div class="nav-item nav-link">
            <github-stars />
          </div>
        </b-navbar-nav>
      </b-collapse>
    </div>
  </b-navbar>
</template>

<style lang="scss" scoped>
.app-navbar {
  background-color: $blue;
}
</style>

<script>
import { mapGetters } from 'vuex'
import GithubStars from '~/components/GithubStars.vue'
import { version, nav } from '~/content'

export default {
  components: {
    GithubStars
  },
  computed: {
    ...mapGetters(['isAuthenticated', 'user']),
    nav: () => nav,
    version: () => version
  }
}
</script>
