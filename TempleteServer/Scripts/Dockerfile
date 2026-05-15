FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 호스트 기준: docker_build/Server/GameServer/GameServer.csproj
COPY ["Server/GameServer/GameServer.csproj", "Server/GameServer/"]
RUN dotnet restore "Server/GameServer/GameServer.csproj"

COPY . .

WORKDIR "/src/Server/GameServer"
RUN dotnet build "GameServer.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/Server/GameServer"
RUN dotnet publish "GameServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameServer.dll"]