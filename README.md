# PStandings
PCMS Contest Explorer

Rewrite of [pcms-standing-parser](https://github.com/alikhil/pcms-standing-parser) in dotnet core + React.js

## Fast Start

Build all services:

```sh
docker-compose build
```

Run database and apply migrations:

```sh
docker-compose up -d db migrations
```

Wait for a while, then run parser and webapp:

```sh
docker-compose up -d --no-deps parser standings 
```

It will take some time to parse all exiting contest files. But web service will be available.