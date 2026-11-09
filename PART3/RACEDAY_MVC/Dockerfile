# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the MVC project file and restore dependencies
COPY ["RaceDay.Web.csproj", "./"]
RUN dotnet restore "RaceDay.Web.csproj"

# Copy the remaining source code
COPY . .
RUN dotnet publish "RaceDay.Web.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "RaceDay.Web.dll"]