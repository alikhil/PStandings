# PStandings
PCMS Contest Explorer

Rewrite of [pcms-standing-parser](https://github.com/alikhil/pcms-standing-parser) in asp dotnet core + React.ts

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

Open `https://localhost:8080/` in your browser

It will take some time to parse all existing contest files. But web service will be available.

## Configuration

### Samples

Let's say you have configured PCMS to export xml files to some directory, we name it `SAMPLES_DIR`.

Then update parser service `docker-compose.yml`:

```yaml
    volumes:
        - "SAMPLES_DIR:/samples"
```

### Other options

Each of services: Standings, Standings.Parser and Standings.Data contains appsetting.json file containing some configuration options. You can update them inplace and rebuild images.

Here is docker version of config for Parser:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "Parsing": {
    "XmlDirectory": "/samples/",
    "ContestStartTime": 1475311854218,
    "RefreshDelay": 15
  },
  "ConnectionStrings": {
    "StandingsConnectionString": "Server=db;Database=master;User=sa;Password=myP@ssw0rd;"
  }
}
```

#### RefreshDelay

Delay in seconds after which parser starts parsing from the scratch.

#### ContestStartTime

Unix timestamp in ms.

Since all timestamps in PCMS's xml files are relative to contest start time. I decided to export time of contest start as a constant to configs. 