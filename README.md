# BookGenerator
![Build workflow](https://github.com/Mental-NV/BookGenerator/actions/workflows/publishwebapp.yml/badge.svg)

Generates books by a title using ChatGPT capabilities.

Published on https://bookgenerator.azurewebsites.net/
## Getting Started

### Database Migration

Run Entity Framework migrations for the Persistence project:

```powershell
dotnet ef database update --project .\src\BookGenerator.WebApi\BookGenerator.WebApi.csproj
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
