import { toQueryString } from '~/assets/js/urls'
let axios

export function setClient(axiosClient) {
  axios = axiosClient
}

export const getBookmarks = async (skip, take, q) => {
  const qs = toQueryString(
    {
      skip: skip || 0,
      take: take || 100,
      q: q || ''
    },
    false
  )
  return (await axios.get(`/api/bookmarks?${qs}`)).data
}

export const getBookmark = async id => {
  return (await axios.get(`/api/bookmarks/${id}`)).data
}

export const addBookmark = async bookmark => {
  return (await axios.post(`/api/bookmarks`, bookmark)).data
}

export const updateBookmark = async bookmark => {
  return (await axios.put(`/api/bookmarks/${bookmark.id}`, {
    url: bookmark.url
  })).data
}

export const deleteBookmark = async id => {
  return (await axios.delete(`/api/bookmarks/${id}`)).data
}
