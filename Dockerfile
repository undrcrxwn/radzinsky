FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY /src .
WORKDIR "/Radzinsky.Bot"
RUN dotnet restore "Radzinsky.Bot.csproj"
RUN dotnet build "Radzinsky.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Radzinsky.Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Radzinsky.Bot.dll"]