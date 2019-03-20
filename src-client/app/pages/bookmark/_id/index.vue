<template>
  <section class="bookmark-section">
    <div class="row mb-3">
      <div class="col-auto mr-auto"></div>
      <div class="col-auto">
        <b-button-toolbar v-if="isMyBookmark">
          <b-button-group size="sm" class="ml-1">
            <b-button
              variant="primary"
              :disabled="working"
              @click.stop="updateBookmark"
              ><i class="fa fa-pencil-square-o" aria-hidden="true"></i>
              Update</b-button
            >
          </b-button-group>
          <b-button-group size="sm" class="ml-1">
            <b-button
              variant="danger"
              :disabled="working"
              @click.stop="deleteBookmark"
              ><i class="fa fa-trash-o" aria-hidden="true"></i> Delete</b-button
            >
          </b-button-group>
        </b-button-toolbar>
      </div>
    </div>
    <div class="small">
      <pre class="small"
        >{{ bookmarkJson }}
      </pre>
    </div>
  </section>
</template>

<script>
import { mapGetters } from 'vuex'
import { updateBookmark, deleteBookmark } from '~/assets/js/api'

export default {
  data() {
    return {
      working: false
    }
  },
  computed: {
    ...mapGetters(['isAuthenticated', 'user', 'bookmarks']),
    bookmarkId() {
      return this.$route.params.id
    },
    bookmark() {
      return this.bookmarks.find(b => b.id === this.bookmarkId) // || this.freshBookmark
    },
    isMyBookmark() {
      return (
        this.isAuthenticated &&
        this.user &&
        this.bookmark &&
        this.bookmark.createdBy.id === this.user.id
      )
    },
    bookmarkJson() {
      return JSON.stringify(this.bookmark || {}, null, '  ')
    }
  },
  async created() {
    if (this.bookmarks.length === 0) {
      await this.$store.dispatch('fetchBookmarks')
    }
  },
  methods: {
    async updateBookmark() {
      this.working = true
      try {
        await updateBookmark(this.bookmark)
      } catch {}
      this.working = false
    },
    async deleteBookmark() {
      this.working = true
      try {
        await deleteBookmark(this.bookmarkId)
      } catch {}
      this.working = false
      this.$router.push('/')
    }
  }
}
</script>
