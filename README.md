# BookGenerator
![Build workflow](https://github.com/Mental-NV/BookGenerator/actions/workflows/publish.yml/badge.svg) ![Code Coverage](https://img.shields.io/endpoint?url=https://raw.githubusercontent.com/Mental-NV/BookGenerator/master/.github/badges/coverage.json)

Generates books by a title using ChatGPT capabilities.

Published on https://bookgenerator.azurewebsites.net/

## Getting Started

### Database Migration

Run Entity Framework migrations for the Persistence project:

```powershell
dotnet ef database update --project .\src\BookGenerator.Persistence\BookGenerator.Persistence.csproj --startup-project .\src\BookGenerator.WebApi\BookGenerator.WebApi.csproj
```

### Web API

Build and run the Web API:

```powershell
# Build
dotnet build

# Test
dotnet test

# Run
dotnet run --project .\src\BookGenerator.WebApi\BookGenerator.WebApi.csproj
```

### Client SPA

Build and run the Client SPA application:

```powershell
# Run
dotnet run --project .\src\BookGenerator.ClientSpa\BookGenerator.ClientSpa.csproj
```

#### React + Vite Client App

Build and test the React + Vite frontend application:

```powershell
# Navigate to the ClientApp directory
cd .\src\BookGenerator.ClientSpa\ClientApp

# Install dependencies
npm install

# Development server
npm run dev

# Build for production
npm run build

# Run tests
npm run test
```

### API Integration Tests (Bruno)

Run the API integration tests using Bruno CLI:

#### Install Bruno CLI

```powershell
npm install -g @usebruno/cli
```

#### Run Cloud Tests (CI/CD behavior)

```powershell
cd tests/Bruno/BookGenerator
bru run --env cloud
```

#### Run Local Tests (`https://localhost:7445`)

```powershell
# One-time: trust local development certificate
dotnet dev-certs https --trust

# Start WebApi in deterministic local mode
$env:Model='Test'
dotnet run --project .\src\BookGenerator.WebApi\BookGenerator.WebApi.csproj

# In a separate terminal, run Bruno locally
cd tests/Bruno/BookGenerator
bru run --env local
```

Optional troubleshooting (if local TLS trust is not set up yet):

```powershell
bru run --env local --insecure
```

## SQLite Deployment Notes (Azure App Service Linux)

- The Web API now uses SQLite and auto-applies EF Core migrations on startup.
- Store the SQLite file outside the deployment folder so redeployments do not overwrite it.
- Recommended Azure App Service setting (Web API app):
  - `BookGeneratorOptions__DatabaseConnectionString=Data Source=/home/data/bookgenerator/bookgenerator.db;Cache=Shared;Pooling=True`
- Keep the Web API App Service scaled to `1` instance while using SQLite.