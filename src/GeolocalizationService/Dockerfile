FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY src/Contracts/Contracts.csproj src/Contracts/
COPY src/GeolocalizationService/Domain/Domain.csproj src/GeolocalizationService/Domain/
COPY src/GeolocalizationService/Application/Application.csproj src/GeolocalizationService/Application/
COPY src/GeolocalizationService/Infra/Infra.csproj src/GeolocalizationService/Infra/
COPY src/GeolocalizationService/Api/Api.csproj src/GeolocalizationService/Api/

RUN dotnet restore src/GeolocalizationService/Api/Api.csproj

COPY src/Contracts/ src/Contracts/
COPY src/GeolocalizationService/ src/GeolocalizationService/

RUN dotnet publish src/GeolocalizationService/Api/Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
