<template>
  <section class="bookmarks-section">
    <div class="row mb-3">
      <div class="col-auto mr-auto">
        <b-button-toolbar
          aria-label="Toolbar with button groups and input groups"
        >
          <b-input-group size="sm" prepend="Search">
            <b-form-input v-model="searchText" class="text-left" />
          </b-input-group>
        </b-button-toolbar>
      </div>
      <div class="col-auto">
        <b-button-toolbar v-if="isAuthenticated">
          <b-button-group size="sm" class="ml-1">
            <b-button v-b-modal.addBookmarkModal variant="primary"
              ><i class="fa fa-plus-circle" aria-hidden="true"></i> Add
              <span class="d-none d-sm-inline-block">Bookmark</span></b-button
            >
          </b-button-group>
        </b-button-toolbar>
      </div>
    </div>
    <b-spinner v-show="!initialized" class="mt-3" />
    <b-alert :show="initialized && bookmarks.length === 0" variant="info">
      There are no bookmarks in the database.
    </b-alert>
    <b-card-group deck class="justify-content-center">
      <app-bookmark
        v-for="bookmark in limitBy(filterBy(bookmarks, searchText), 100)"
        :key="bookmark.id"
        :bookmark="bookmark"
      />
    </b-card-group>
    <b-modal
      id="addBookmarkModal"
      ref="addBookmarkModal"
      title="Add Bookmark"
      cancel-variant="link"
      :ok-disabled="!newUrlState && addingBookmark"
      :no-close-on-esc="addingBookmark"
      :no-close-on-backdrop="addingBookmark"
      :hide-header-close="addingBookmark"
      @ok.prevent="onAddBookmark"
      @hidden.prevent="onResetAddBookmark"
    >
      <b-spinner v-show="addingBookmark" label="Adding bookmark ..." />
      <b-form
        v-show="!addingBookmark"
        v-if="showAddBookmarkForm"
        ref="addBookmarkForm"
        @submit.prevent
      >
        <b-form-group
          id="urlInputGroup"
          label="Url to bookmark:"
          label-for="urlInput"
          description="The url you would like to bookmark."
        >
          <b-form-input
            id="urlInput"
            v-model="form.url"
            type="text"
            required
            :state="newUrlState"
            placeholder="Example: https://www.google.com"
            aria-describedby="urlInputFeedback"
          />
          <b-form-invalid-feedback id="urlInputFeedback">
            Must be a valid url
          </b-form-invalid-feedback>
        </b-form-group>

        <b-alert v-model="hasErrorMessage" variant="danger" dismissible>
          {{ errorMessage }}
        </b-alert>
      </b-form>
    </b-modal>
  </section>
</template>

<script>
import { mapGetters } from 'vuex'
import { addBookmark } from '~/assets/js/api'
import AppBookmark from '~/components/AppBookmark.vue'

export default {
  components: {
    AppBookmark
  },
  data() {
    return {
      initialized: false,
      searchText: '',
      form: {
        url: ''
      },
      // bookmarks: [],
      errorMessage: null,
      showAddBookmarkForm: true,
      addingBookmark: false
    }
  },
  computed: {
    ...mapGetters(['isAuthenticated', 'user', 'bookmarks']),
    newUrlState() {
      return this.isValidUrl(this.form.url)
    },
    hasErrorMessage() {
      return this.errorMessage && this.errorMessage.length > 0
    }
  },
  async created() {
    await this.$store.dispatch('fetchBookmarks')
    // this.errorMessage = null
    // try {
    //   const bookmarks = (await this.$axios.get(
    //     '/api/bookmarks?skip=0&take=100'
    //   )).data
    //   this.bookmarks.splice(0, this.bookmarks.length, ...bookmarks)
    // } catch (err) {
    //   if (err.response && err.response.data && err.response.data.message) {
    //     this.errorMessage = err.response.data.message
    //   } else {
    //     this.errorMessage = JSON.stringify(err.response)
    //   }
    // }
    this.initialized = true
  },
  methods: {
    async onAddBookmark() {
      this.errorMessage = null
      this.addingBookmark = true
      try {
        await addBookmark(this.form)
        // const bookmark = (await this.$axios.post('/api/bookmarks', this.form))
        //   .data
        // this.bookmarks.splice(0, 0, bookmark)
        this.addingBookmark = false
        this.$refs.addBookmarkModal.hide()
      } catch (err) {
        this.addingBookmark = false
        if (err.response && err.response.data && err.response.data.message) {
          this.errorMessage = err.response.data.message
        } else {
          this.errorMessage = JSON.stringify(err.response)
        }
      }
    },
    onResetAddBookmark() {
      this.errorMessage = null
      this.form.url = ''
      this.showAddBookmarkForm = false
      this.$nextTick(() => {
        this.showAddBookmarkForm = true
      })
    },
    isValidUrl(str) {
      // from https://stackoverflow.com/questions/5717093/check-if-a-javascript-string-is-a-url
      const pattern = new RegExp(
        '^(https?:\\/\\/)?' + // protocol
        '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|' + // domain name
        '((\\d{1,3}\\.){3}\\d{1,3}))' + // OR ip (v4) address
        '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port and path
        '(\\?[;&a-z\\d%_.~+=-]*)?' + // query string
          '(\\#[-a-z\\d_]*)?$',
        'i'
      ) // fragment locator
      return !!pattern.test(str)
    }
  }
}
</script>
