# Sử dụng hình ảnh ASP.NET Core làm cơ sở
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Sử dụng hình ảnh .NET SDK để xây dựng ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Hairhub.API/Hairhub.API.csproj", "Hairhub.API/"]
COPY ["Hairhub.Domain/Hairhub.Domain.csproj", "Hairhub.Domain/"]
COPY ["Hairhub.Infrastructure/Hairhub.Infrastructure.csproj", "Hairhub.Infrastructure/"]
COPY ["Hairhub.Service/Hairhub.Service.csproj", "Hairhub.Service/"]
RUN dotnet restore "Hairhub.API/Hairhub.API.csproj"
COPY . .
WORKDIR "/src/Hairhub.API"
RUN dotnet build "Hairhub.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Hairhub.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Tạo hình ảnh cuối cùng để chạy ứng dụng
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hairhub.API.dll"]

# Đặt biến môi trường để xác định URL mà ứng dụng sẽ lắng nghe
ENV ASPNETCORE_URLS="https://+:443;http://+:80"
