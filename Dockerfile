# =========================
# Build Stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy project files first
COPY ["Nexvara.WebAPI/Nexvara.WebAPI.csproj", "Nexvara.WebAPI/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Nexvara.WebAPI/Nexvara.WebAPI.csproj"

# Copy complete source
COPY . .

# Build
WORKDIR "/src/Nexvara.WebAPI"
RUN dotnet build "Nexvara.WebAPI.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "Nexvara.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false


# =========================
# Runtime Stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

# Render uses PORT environment variable.
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-10000}

EXPOSE 10000

ENTRYPOINT ["dotnet", "Nexvara.WebAPI.dll"]