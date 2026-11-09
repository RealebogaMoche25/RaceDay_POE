# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the API project file and restore dependencies
COPY ["RACEDAY_API.csproj", "./"]
RUN dotnet restore "RACEDAY_API.csproj"

# Copy the remaining source code
COPY . .
RUN dotnet publish "RACEDAY_API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "RACEDAY_API.dll"]