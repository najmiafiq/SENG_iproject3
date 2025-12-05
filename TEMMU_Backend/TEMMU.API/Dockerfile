# Stage 1: Build the application
# Use the official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all project files and restore dependencies
# This is crucial for multi-project solutions (API, Core, Infrastructure)
COPY ["TEMMU.API/TEMMU.API.csproj", "TEMMU.API/"]
COPY ["TEMMU.Core/TEMMU.Core.csproj", "TEMMU.Core/"]
COPY ["TEMMU.Infrastructure/TEMMU.Infrastructure.csproj", "TEMMU.Infrastructure/"]
RUN dotnet restore "TEMMU.API/TEMMU.API.csproj"

# Copy all source code
COPY . .

# Change to the API project directory and publish the app
WORKDIR /src/TEMMU.API
RUN dotnet publish "TEMMU.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Create the final runtime image
# Use the official lightweight ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Expose the port the application will listen on
EXPOSE 8080 

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Define the entry point for the container
ENTRYPOINT ["dotnet", "TEMMU.API.dll"]