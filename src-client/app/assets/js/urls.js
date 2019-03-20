import queryString from 'query-string'

// old school parser
// const parseQueryParams = () => {
//   const params = {}
//   window.location.href.replace(
//     /([^(?|#)=&]+)(=([^&]*))?/g,
//     ($0, $1, $2, $3) => {
//       params[$1] = decodeURIComponent($3)
//     }
//   )
//   return params
// }

export const getQueryParams = () => {
  return (
    queryString.parse(window.location.search, { arrayFormat: 'comma' }) || {}
  )
}

export const getQueryParam = str => {
  return getQueryParams()[str]
}

export const toQueryString = (obj, includeQuestionMark) => {
  const stringified = queryString.stringify(obj, { arrayFormat: 'comma' })
  if (stringified && stringified.length > 0) {
    return (includeQuestionMark ? '?' : '') + stringified
  }
  return ''
}
