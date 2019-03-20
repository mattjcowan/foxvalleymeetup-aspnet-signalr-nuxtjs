import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr'

import { getToken } from '~/assets/js/auth'

export default ({ app, store }, inject) => {
  const hub = new HubConnectionBuilder()
    .withUrl('/hub', {
      accessTokenFactory: function() {
        console.log('Getting token for hub')
        return getToken()
      }
    })
    .configureLogging(LogLevel.Information)
    .build()

  inject('hub', hub)

  hub.on('Connected', message => {
    console.info('Connected to SignalR Hub.', message)
  })

  hub.on('Disconnected', message => {
    console.warn('Disconnected from SignalR Hub.', message)
  })

  hub.on('ReceiveMessage', res => {
    console.log('Received message from signalr', JSON.stringify(res))
    // const { audience, group, action, data } = res
    const { action, data } = res
    switch (action) {
      case 'bookmark_created':
      case 'bookmark_updated':
        store.commit('addOrUpdateBookmark', data.bookmark)
        break
      case 'bookmark_deleted':
        store.commit('removeBookmark', data.bookmark.id)
        break
      default:
        break
    }
  })

  hub.start().catch(function(err) {
    return console.error(err)
  })

  // with reconnect capability (async/await, not IE11 compatible)
  async function start() {
    try {
      console.log('Attempting reconnect')
      await hub.start()
    } catch (err) {
      console.log(err)
      setTimeout(() => start(), 5000)
    }
  }

  hub.onclose(async () => {
    await start()
  })

  // sample sending of message
  /*
  hub
    .invoke('SendMessageToOthers', {
      action: action,
      data: Object.assign(
        { user: get(store, 'getters.loggedUser.name') },
        message
      )
    })
    .catch(err => console.error(err.toString()))
  */
}
