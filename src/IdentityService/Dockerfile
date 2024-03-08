#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
<<<<<<< HEAD
COPY ["src/IdentityService/IdentityService.csproj", "src/IdentityService/"]
RUN dotnet restore "./src/IdentityService/./IdentityService.csproj"
COPY . .
WORKDIR "/src/src/IdentityService"
=======
COPY ["IdentityService/IdentityService.csproj", "IdentityService/"]
RUN dotnet restore "./IdentityService/./IdentityService.csproj"
COPY . .
WORKDIR "/src/IdentityService"
>>>>>>> dc507927a9b31ee79b81d00e7d9d1faacc71d3fd
RUN dotnet build "./IdentityService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IdentityService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.dll"]