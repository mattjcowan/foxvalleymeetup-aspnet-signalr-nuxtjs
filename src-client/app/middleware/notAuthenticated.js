export default function({ store, redirect }) {
  console.log('middleware.notAuthenticated', store.getters.isAuthenticated)
  if (store.getters.isAuthenticated) {
    return redirect('/')
  }
}
