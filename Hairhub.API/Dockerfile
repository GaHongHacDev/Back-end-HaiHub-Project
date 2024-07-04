#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Hairhub.API/Hairhub.API.csproj", "Hairhub.API/"]
COPY ["Hairhub.Domain/Hairhub.Domain.csproj", "Hairhub.Domain/"]
COPY ["Hairhub.Infrastructure/Hairhub.Infrastructure.csproj", "Hairhub.Infrastructure/"]
COPY ["Hairhub.Service/Hairhub.Service.csproj", "Hairhub.Service/"]
RUN dotnet restore "./Hairhub.API/Hairhub.API.csproj"
COPY . .
WORKDIR "/src/Hairhub.API"
RUN dotnet build "./Hairhub.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Hairhub.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY /etc/letsencrypt/live/gahonghac.net/privkey.pem /etc/letsencrypt/live/gahonghac.net/privkey.pem
COPY /etc/letsencrypt/live/gahonghac.net/fullchain.pem /etc/letsencrypt/live/gahonghac.net/fullchain.pem
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hairhub.API.dll"]