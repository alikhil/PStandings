
FROM microsoft/dotnet:2.0.3-sdk
WORKDIR /app

COPY Standings.Data/*.csproj ./

COPY Standings.Data ./

ENTRYPOINT ["/bin/bash", "/app/run.sh"]

