import Vue from 'vue'
import Vue2Filters from 'vue2-filters'
import moment from 'moment'

Vue.use(Vue2Filters)
Vue.mixin(Vue2Filters.mixin)

// see https://devhints.io/moment
Vue.filter('formatDate', function(value) {
  if (value) {
    return moment.utc(String(value)).format('llll')
  }
})
Vue.filter('fromNow', function(value) {
  if (value) {
    return moment.utc(String(value)).fromNow()
  }
})
