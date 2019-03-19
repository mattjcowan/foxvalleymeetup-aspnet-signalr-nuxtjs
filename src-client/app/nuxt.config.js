// import axios from 'axios'
import pkg from './package'

// disregard SSL cert errors while developing
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0'

const apiUrl = process.env.API_URL || 'http://localhost:5000'
const apiOptions = {
  target: apiUrl,
  secure: false,
  changeOrigin: true,
  ws: false,
  autoRewrite: true
}

export default {
  mode: 'spa',

  /*
   ** Headers of the page
   */
  head: {
    title: pkg.name,
    meta: [
      { charset: 'utf-8' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1' },
      { hid: 'description', name: 'description', content: pkg.description },
      { name: 'msapplication-TileColor', content: '#da532c' },
      { name: 'theme-color', content: '#ffffff' }
    ],
    link: [
      // { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
      {
        rel: 'apple-touch-icon',
        sizes: '120x120',
        href: '/apple-touch-icon.png'
      },
      {
        rel: 'icon',
        type: 'image/png',
        sizes: '32x32',
        href: '/favicon-32x32.png'
      },
      {
        rel: 'icon',
        type: 'image/png',
        sizes: '16x16',
        href: '/favicon-16x16.png'
      },
      { rel: 'manifest', href: '/site.webmanifest' },
      { rel: 'mask-icon', href: '/safari-pinned-tab.svg', color: '#5bbad5' }
    ]
  },

  /*
   ** Customize the progress-bar color
   */
  loading: { color: '#fff' },

  /*
   ** Global CSS
   */
  css: ['@/assets/scss/main.scss'],

  /*
   ** Plugins to load before mounting the App
   */
  plugins: ['~/plugins/extendAxios'],

  /*
   ** Nuxt.js modules
   */
  modules: [
    // Doc: https://axios.nuxtjs.org/usage
    '@nuxtjs/axios',
    'bootstrap-vue/nuxt',
    '@nuxtjs/style-resources'
  ],

  /*
   ** Axios module configuration
   */
  axios: {
    // See https://github.com/nuxt-community/axios-module#options
    progress: true,
    proxy: true
  },

  bootstrapVue: {
    bootstrapCSS: false, // or `css`
    bootstrapVueCSS: false // or `bvCSS`
  },

  styleResources: {
    scss: ['~assets/scss/_variables.scss', '~assets/scss/_mixins.scss']
  },

  proxy: {
    '/api': apiOptions,
    '/swagger': apiOptions,
    '/hub': Object.assign(apiOptions, { ws: true }) // signalr endpoint
  },

  /*
   ** Build configuration
   */
  build: {
    extractCSS: true,
    splitChunks: {
      layouts: true
    },

    /*
     ** You can extend webpack config here
     */
    extend(config, ctx) {
      // Run ESLint on save
      if (ctx.isDev && ctx.isClient) {
        config.module.rules.push({
          enforce: 'pre',
          test: /\.(js|vue)$/,
          loader: 'eslint-loader',
          exclude: /(node_modules)/
        })
      }
    }
  },

  generate: {
    // dir: 'dist',
    // routes: function () {
    //   return axios.get('https://goodreads/bookmarks').then(res => {
    //     return res.data.map(bookmark => {
    //       return '/bookmarks/' + bookmark.id
    //     })
    //   })
    // }
  }
}
