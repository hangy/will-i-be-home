FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["WillIBeHome.Trainer/WillIBeHome.Trainer.csproj", "WillIBeHome.Trainer/"]
RUN dotnet restore "WillIBeHome.Trainer/WillIBeHome.Trainer.csproj"
COPY . .
WORKDIR "/src/WillIBeHome.Trainer"
RUN dotnet build "WillIBeHome.Trainer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WillIBeHome.Trainer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WillIBeHome.Trainer.dll"]
