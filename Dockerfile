# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/HospitalQueueSystem.API/HospitalQueueSystem.API.csproj", "HospitalQueueSystem.API/"]
COPY ["src/HospitalQueueSystem.Models/HospitalQueueSystem.Models.csproj", "HospitalQueueSystem.Models/"]

# Restore dependencies
RUN dotnet restore "HospitalQueueSystem.API/HospitalQueueSystem.API.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/HospitalQueueSystem.API"
RUN dotnet build "HospitalQueueSystem.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "HospitalQueueSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/api/health || exit 1

# Expose port
EXPOSE 8080

# Set environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run
ENTRYPOINT ["dotnet", "HospitalQueueSystem.API.dll"]