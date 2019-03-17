# foxvalleymeetup-aspnet-signalr-nuxtjs

Building Apps with VueJS, NuxtJS, SignalR and .NET core

## Pre-requisites

- NodeJS: v10.15.3 @ https://nodejs.org
- .NET Core: v 2.2 SDK @ https://dotnet.microsoft.com/download

## Base setup

Install and run server at http://localhost:5000

```sh
mkdir src-server
cd src-server
dotnet new web -n app
cd app
dotnet watch run
```

Install and run client at http://localhost:3000

```sh
mkdir src-client
cd src-client
npx create-nuxt-app app
cd app
npm run dev
```
