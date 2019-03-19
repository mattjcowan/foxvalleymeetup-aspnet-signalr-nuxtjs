// import https from 'https'

export default function({ $axios, store }) {
  // const agent = new https.Agent({
  //   rejectUnauthorized: false
  // })
  $axios.onRequest(config => {
    console.log(config)
    if (process.env.dev) {
      // config.httpsAgent = agent
    }
  })
}
