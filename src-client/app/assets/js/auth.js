import Cookie from 'js-cookie'
import jwtDecode from 'jwt-decode'

export const setToken = (token, persistent) => {
  if (process.server) return
  const options = persistent ? { expires: 7 } : {}
  Cookie.set('jwt', token, options)

  // inspect the token for fun
  const tokenContents = jwtDecode(token)
  console.log('tokenContents', JSON.stringify(tokenContents))
}

export const unsetToken = () => {
  if (process.server) return
  Cookie.remove('jwt')
}

export const getToken = () => {
  if (process.server) return null
  return Cookie.get('jwt')
}
