#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Game.Api.csproj", "Game.Api/"]
RUN dotnet restore "Game.Api/Game.Api.csproj"
COPY . .
WORKDIR "/src/Game.Api"
RUN dotnet build "Game.Api.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "Game.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Game.Api.dll"]
