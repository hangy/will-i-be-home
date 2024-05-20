FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0.300 AS build
WORKDIR /src
COPY ["WillIBeHome.Api/WillIBeHome.Api.csproj", "WillIBeHome.Api/"]
RUN dotnet restore "WillIBeHome.Api/WillIBeHome.Api.csproj"
COPY . .
WORKDIR "/src/WillIBeHome.Api"
RUN dotnet build "WillIBeHome.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WillIBeHome.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WillIBeHome.Api.dll"]
