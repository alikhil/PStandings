
FROM microsoft/dotnet:2.1-runtime-bionic
WORKDIR /app

COPY Standings.Data/*.csproj ./

COPY Standings.Data ./

ENTRYPOINT ["/bin/bash", "/app/run.sh"]

