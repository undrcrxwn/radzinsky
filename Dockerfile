FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/ .
RUN dotnet build Radzinsky.Host/Radzinsky.Host.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Radzinsky.Host/Radzinsky.Host.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Radzinsky.Host.dll"]