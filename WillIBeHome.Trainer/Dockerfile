FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0.204 AS build
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
