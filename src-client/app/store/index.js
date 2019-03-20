import { getBookmarks } from '~/assets/js/api'

export const state = () => {
  return {
    user: null,
    bookmarks: []
  }
}

export const mutations = {
  setUser(state, user) {
    state.user = user || null
  },
  setBookmarks(state, bookmarks) {
    state.bookmarks = bookmarks || []
  },
  addOrUpdateBookmark(state, bookmark) {
    const bookmarkIndex = state.bookmarks.findIndex(b => b.id === bookmark.id)
    if (bookmarkIndex > -1) state.bookmarks.splice(bookmarkIndex, 1, bookmark)
    else state.bookmarks.splice(0, 0, bookmark)
  },
  removeBookmark(state, bookmarkId) {
    const bookmarkIndex = state.bookmarks.findIndex(b => b.id === bookmarkId)
    if (bookmarkIndex > -1) state.bookmarks.splice(bookmarkIndex, 1)
  }
}

export const getters = {
  // someGetter(state, getters, rootState, rootGetters) {}
  user: state => state.user,
  isAuthenticated(state, getters) {
    return !!getters.user
  },
  bookmarks: state => state.bookmarks
}

export const actions = {
  // someAction({ dispatch, commit, getters, rootGetters }, payload) {}
  async fetchBookmarks({ commit }) {
    const bookmarks = await getBookmarks(0, 100, '')
    commit('setBookmarks', bookmarks)
  }
}
