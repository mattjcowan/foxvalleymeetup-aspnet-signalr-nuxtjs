# foxvalleymeetup-aspnet-signalr-nuxtjs

Building Apps with VueJS, NuxtJS, SignalR and .NET core

## Pre-requisites

- NodeJS: v10.15.3 @ https://nodejs.org
- .NET Core: v 2.2 SDK @ https://dotnet.microsoft.com/download

## Server setup

Install and run server at http://localhost:5000

```sh
mkdir src-server
cd src-server
dotnet new web -n app
cd app
dotnet watch run
```

### .NET core authentication

Add packages and `Startup.cs` code

```sh
cd src-server/app
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Swashbuckle.AspNetCore
```

### .NET EF Migrations

To add migrations

```sh
dotnet ef migrations add NAME_FOR_MIGRATIONS
```

## Client setup

Install and run client at http://localhost:3000

```sh
mkdir src-client
cd src-client
npx create-nuxt-app app
cd app
npm run dev
```