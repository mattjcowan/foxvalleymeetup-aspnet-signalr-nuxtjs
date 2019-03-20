import { getToken, unsetToken } from '~/assets/js/auth'

export default function({ isServer, store, req }) {
  // If nuxt generate, disregard this middleware
  if (isServer && !req) return
  const token = getToken()
  if (!token || !store.getters.isAuthenticated) {
    unsetToken()
    store.commit('setUser', null)
  }
}
