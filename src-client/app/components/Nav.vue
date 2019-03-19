<template>
  <!-- <b-navbar toggleable="md" class="navbar-dark bd-navbar" sticky>
    <b-navbar-toggle target="bd-main-nav" />

    <b-navbar-brand to="/" exact>
      <img class="d-block" height="65" src="/logo.svg" />
      <span class="sr-only">Home</span>
    </b-navbar-brand>

    <b-collapse id="bd-main-nav" is-nav class="justify-content-between">
      <b-navbar-nav>
        <b-nav-item to="/" exact active-class="active">Home</b-nav-item>
        <b-nav-item href="/swagger/" active-class="active">Swagger</b-nav-item>
      </b-navbar-nav>

      <b-navbar-nav>
        <b-nav-item-dropdown :text="user" right>
          <b-dropdown-item href="https://bootstrap-vue.js.org"
            >Profile</b-dropdown-item
          >
          <b-dropdown-item href="https://bootstrap-vue.netlify.com"
            >Logou</b-dropdown-item
          >
        </b-nav-item-dropdown>

        <div class="nav-item nav-link">
          <iframe
            src="https://ghbtns.com/github-btn.html?user=mattjcowan&repo=servicestack-nuxt-example&type=star&count=true"
            frameborder="0"
            scrolling="0"
            width="100px"
            height="20px"
          />
        </div>
      </b-navbar-nav>
    </b-collapse>
  </b-navbar> -->
  <b-navbar
    toggleable="lg"
    type="dark"
    variant="primary"
    class="app-navbar"
    sticky
  >
    <b-navbar-brand to="/" exact>
      <img class="d-block" height="65" src="/logo.svg" />
      <span class="sr-only">Home</span>
    </b-navbar-brand>

    <b-navbar-toggle target="nav_collapse" />

    <b-collapse id="nav_collapse" is-nav>
      <b-navbar-nav>
        <b-nav-item to="/" exact active-class="active">Home</b-nav-item>
        <b-nav-item href="/swagger/" active-class="active">Swagger</b-nav-item>
      </b-navbar-nav>

      <!-- Right aligned nav items -->
      <b-navbar-nav class="ml-auto">
        <b-nav-item-dropdown right>
          <!-- Using button-content slot -->
          <template slot="button-content"
            ><em>{{ user }}</em></template
          >
          <b-dropdown-item href="#" disabled>Profile</b-dropdown-item>
          <b-dropdown-item to="/logout">Logout</b-dropdown-item>
        </b-nav-item-dropdown>

        <div class="nav-item nav-link">
          <iframe
            src="https://ghbtns.com/github-btn.html?user=mattjcowan&repo=servicestack-nuxt-example&type=star&count=true"
            frameborder="0"
            scrolling="0"
            width="100px"
            height="20px"
          />
        </div>
      </b-navbar-nav>
    </b-collapse>
  </b-navbar>
</template>

<style lang="scss" scoped>
.app-navbar {
  background-color: $blue;
}
</style>

<script>
import { version, nav } from '~/content'

export default {
  data() {
    return {
      user: 'mattjcowan'
    }
  },
  computed: {
    nav: () => nav,
    version: () => version
  }
}
</script>
