<template>
  <b-card no-body tag="article" title-tag="h5" class="bookmark-card mb-3">
    <b-card-img-lazy :src="image" :alt="bookmark.title" :fluid="true" top />

    <b-card-body>
      <!-- eslint-disable-next-line vue/no-v-html -->
      <b-card-title v-html="bookmark.title" />
      <!-- <b-card-sub-title>{{ bookmark.title }}</b-card-sub-title> -->
      <b-card-text class="small">
        {{ description | truncate(150) }}
      </b-card-text>
    </b-card-body>
    <b-card-footer class="p-3 small justify-content-center">
      <b-link
        :href="bookmark.url"
        variant="link"
        target="_blank"
        class="card-link"
        ><i class="fa fa-external-link" aria-hidden="true"></i> Website</b-link
      >
      <b-link :to="`/bookmark/${bookmark.id}`" variant="link" class="card-link"
        ><i class="fa fa-eye" aria-hidden="true"></i> Details</b-link
      >
    </b-card-footer>
    <div slot="footer" class="small">
      <span class="text-muted small"
        >Created {{ bookmark.createDate | fromNow }} by
        <strong
          >{{ bookmark.createdBy.firstName }}
          {{ bookmark.createdBy.lastName.substr(0, 1) }}.</strong
        ></span
      >
    </div>
  </b-card>
</template>

<script>
export default {
  props: {
    bookmark: { type: Object, required: true }
  },
  computed: {
    description() {
      if (this.bookmark.metaTags) {
        const altDescription = this.bookmark.metaTags.filter(d => {
          return (
            d.property === 'og:description' ||
            d.property === 'twitter:description'
          )
        })
        if (altDescription.length > 0) return altDescription[0].content
      }
      return this.bookmark.description || ''
    },
    image() {
      if (this.bookmark.metaTags) {
        const altImage = this.bookmark.metaTags.filter(d => {
          return d.property === 'og:image' || d.property === 'twitter:image'
        })
        if (altImage.length > 0) return altImage[0].content
      }
      return this.bookmark.screenshot || 'https://picsum.photos/300/150/?random' // ?image=25'
    }
  }
}
</script>
