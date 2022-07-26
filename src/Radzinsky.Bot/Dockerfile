FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Radzinsky.Bot/Radzinsky.Bot.csproj", "Radzinsky.Bot/"]
COPY ["src/Radzinsky.Application/Radzinsky.Application.csproj", "Radzinsky.Application/"]
COPY ["src/Radzinsky.Domain/Radzinsky.Domain.csproj", "Radzinsky.Domain/"]
COPY ["src/Radzinsky.Infrastructure/Radzinsky.Infrastructure.csproj", "Radzinsky.Infrastructure/"]
RUN dotnet restore "src/Radzinsky.Bot/Radzinsky.Bot.csproj"
COPY . .
WORKDIR "/src/Radzinsky.Bot"
RUN dotnet build "Radzinsky.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Radzinsky.Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Radzinsky.Bot.dll"]
