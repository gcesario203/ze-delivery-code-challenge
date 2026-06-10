FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY src/Contracts/Contracts.csproj src/Contracts/
COPY src/ApiGateway/ApiGateway.csproj src/ApiGateway/

RUN dotnet restore src/ApiGateway/ApiGateway.csproj

COPY src/Contracts/ src/Contracts/
COPY src/ApiGateway/ src/ApiGateway/

RUN dotnet publish src/ApiGateway/ApiGateway.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
