FROM node:20 AS frontend-build
WORKDIR /app/client

COPY soclover.client/package*.json ./
RUN npm install

COPY soclover.client/ ./
RUN npm run build


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

RUN apt-get update && apt-get install -y curl \
    && curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs \
    && rm -rf /var/lib/apt/lists/*

COPY ["SoClover.Server/SoClover.Server.csproj", "SoClover.Server/"]
RUN dotnet restore "SoClover.Server/SoClover.Server.csproj"

COPY . .
WORKDIR /src/SoClover.Server

COPY --from=frontend-build /app/client/dist /src/SoClover.Server/wwwroot

RUN dotnet publish "SoClover.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SoClover.Server.dll"]