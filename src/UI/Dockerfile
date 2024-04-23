FROM mongo:latest
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UI.csproj", "."]
RUN dotnet restore "UI.csproj"
COPY . .
RUN dotnet build "UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UI.dll"]