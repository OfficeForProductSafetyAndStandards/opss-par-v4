FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Api/Opss.PrimaryAuthorityRegister.Api/Opss.PrimaryAuthorityRegister.Api.csproj", "Api/Opss.PrimaryAuthorityRegister.Api/"]
RUN dotnet restore "./Api/Opss.PrimaryAuthorityRegister.Api/Opss.PrimaryAuthorityRegister.Api.csproj"
COPY . .
WORKDIR "/src/Api/Opss.PrimaryAuthorityRegister.Api"
RUN dotnet build "./Opss.PrimaryAuthorityRegister.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Opss.PrimaryAuthorityRegister.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Opss.PrimaryAuthorityRegister.Api.dll"]