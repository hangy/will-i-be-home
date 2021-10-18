FROM mcr.microsoft.com/dotnet/aspnet:5.0.10-alpine3.13 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0.402-alpine3.13 AS build
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
