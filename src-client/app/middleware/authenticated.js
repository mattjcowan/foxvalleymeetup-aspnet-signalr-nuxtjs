import { toQueryString } from '~/assets/js/urls'

export default function({ store, redirect, route }) {
  console.log('middleware.authenticated', store.getters.isAuthenticated)
  if (!store.getters.isAuthenticated) {
    const q = toQueryString({ redirect: route ? route.path : '' }, true)
    return redirect('/login' + q)
  }
}
