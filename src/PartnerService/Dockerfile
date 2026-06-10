FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY src/Contracts/Contracts.csproj src/Contracts/
COPY src/PartnerService/Domain/Domain.csproj src/PartnerService/Domain/
COPY src/PartnerService/Application/Application.csproj src/PartnerService/Application/
COPY src/PartnerService/Infra/Infra.csproj src/PartnerService/Infra/
COPY src/PartnerService/Api/Api.csproj src/PartnerService/Api/

RUN dotnet restore src/PartnerService/Api/Api.csproj

COPY src/Contracts/ src/Contracts/
COPY src/PartnerService/ src/PartnerService/

RUN dotnet publish src/PartnerService/Api/Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
