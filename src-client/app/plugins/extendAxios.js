import https from 'https'
import { getToken } from '~/assets/js/auth'
import { setClient } from '~/assets/js/api'

const agent = new https.Agent({
  rejectUnauthorized: false
})

export default ({ app: { $axios }, store }) => {
  $axios.onRequest(config => {
    const token = getToken()

    if (token) {
      // eslint-disable-next-line dot-notation
      config.headers.common['Authorization'] = `Bearer ${token}`
    }

    if (process.env.dev) {
      config.httpsAgent = agent
    }
  })

  // little trick
  setClient($axios)
}
