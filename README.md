# foxvalleymeetup-aspnet-signalr-nuxtjs

Building Apps with VueJS, NuxtJS, SignalR and .NET core

This repository is a quick weekend project demo of an app showcasing a variety of features
pertinent to building apps with the technologies listed above. I built this demo for a [local
meetup in my area](https://www.meetup.com/Fox-Valley-NET-Web-Development-Meetup/events/259403666/)

### Screenshots

| Dashboard      | Login      | API Docs    |
|------------|-------------|-------------| 
|  <img src="https://raw.githubusercontent.com/mattjcowan/foxvalleymeetup-aspnet-signalr-nuxtjs/master/screenshots/Screenshot%202019-05-01%2010.04.21.png" width="250"> | <img src="https://raw.githubusercontent.com/mattjcowan/foxvalleymeetup-aspnet-signalr-nuxtjs/master/screenshots/Screenshot%202019-05-01%2010.04.34.png" width="250"> | <img src="https://raw.githubusercontent.com/mattjcowan/foxvalleymeetup-aspnet-signalr-nuxtjs/master/screenshots/Screenshot%202019-05-01%2010.04.53.png" width="250"> |

### Some other stuff:

- [Slide deck](https://slides.com/mattjcowan/foxvalleymeetup-aspnet-signalr-nuxt)
- [Speaker notes](NOTES.md)

### Interesting features in this repo:

- install/setup
    - ubuntu 16.04 vanilla vm bash script setup
    - bash deploy script
- .net core 2.2
    - signalr websockets
    - nodeservices (taking screenshots from c# using phantomjs)
    - htmlagilitypack
    - swagger documentation of api
    - jwt auth
    - script to bump version
- nuxtjs spa
    - jwt auth (token in cookie)
    - customizing bootstrap (via scss)
    - global scss variables / mixins
    - vuex + selective persistence
    - axios + proxy (for api and web sockets on dev)
    - custom middleware
    - custom plugins
    - websocket hub (w/ signalr)
    - dynamic routing (with slug/params)
    - local storage
    - filters
    - static files and adding head links/scripts

