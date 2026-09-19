# --- Stage 1: Build ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
Copy ["ReturnPolicy/ReturnPolicy.API.csproj", "ReturnPolicy/"]
Copy ["ReturnPolicy.Services/ReturnPolicy.Services.csproj", "ReturnPolicy.Services/"]
Copy ["ReturnPolicy.Infrastructure/ReturnPolicy.Infrastructure.csproj", "ReturnPolicy.Infrastructure/"]
RUN dotnet restore "ReturnPolicy/ReturnPolicy.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/ReturnPolicy"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# --- Stage 2: Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ReturnPolicy.API.dll"]