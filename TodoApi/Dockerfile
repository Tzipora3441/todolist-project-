# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore
COPY ["TodoApi.csproj", "./"]
RUN dotnet restore "TodoApi.csproj"

# Copy source code and publish
COPY . .
# השתמשנו ב-publish במקום ב-build כדי להבטיח שכל ה-Dependencies בפנים
RUN dotnet publish "TodoApi.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published output
COPY --from=build /app/publish .

# Render משתמש בפורט 10000 כברירת מחדל, כדאי להתאים את האפליקציה לזה
EXPOSE 10000
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "TodoApi.dll"]