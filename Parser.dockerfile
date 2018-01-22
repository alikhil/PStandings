
FROM microsoft/dotnet:2.0-sdk AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY Standings.Data /Standings.Data

COPY Standings.Parser/*.csproj ./
RUN dotnet restore

# copy everything else and build
COPY Standings.Parser ./
RUN dotnet publish -c Release -o out

# build runtime image
FROM microsoft/dotnet:2.0-runtime 
WORKDIR /app
COPY --from=build-env /app/out ./
COPY Standings.Parser/appsettings.docker.json ./appsettings.json
ENTRYPOINT ["dotnet", "Standings.Parser.dll"]