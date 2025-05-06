# PSK2025

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/) (version 20.12 or later)
- [Docker](https://www.docker.com/products/docker-desktop) for running PostgreSQL and containerized applications

## Getting Started

Follow these steps to get the application running locally:

### 1. Clone the Repository

```bash
git clone https://github.com/canyonhhh/PSK2025
cd PSK2025
```

### 2. Configure Secrets (Development)

Set up JWT secrets for development:

```bash
dotnet user-secrets init --project PSK2025.ApiService
dotnet user-secrets set "JWT:Secret" "YourDevelopmentSecretKey1234567890123456789012" --project PSK2025.ApiService
dotnet user-secrets set "JWT:ValidIssuer" "https://localhost" --project PSK2025.ApiService
dotnet user-secrets set "JWT:ValidAudience" "https://localhost" --project PSK2025.ApiService
dotnet user-secrets set "JWT:ExpirationInDays" "7" --project PSK2025.ApiService
```

### 3. Set Up the Frontend

Navigate to the frontend directory and install dependencies:

```bash
cd PSK2025.Frontend
npm install
```

### 4. Run with .NET Aspire

The application is designed to run with .NET Aspire, which coordinates all services, including:

- PostgreSQL database
- API service
- Migration service
- Frontend application

From the root directory, run:

```bash
dotnet run --project PSK2025.AppHost
```

This command will:

- Start the PostgreSQL container
- Apply database migrations
- Start the API service
- Build and run the frontend

The Aspire dashboard will open in your browser, showing the status of all services. You can access the frontend application and API from there.

## Database Migrations

Migrations are handled automatically by the MigrationService when running with Aspire.

To manually create a new migration after modifying entity models:

```bash
dotnet ef migrations add <MigrationName> --project PSK2025.Data
```

## Testing

Run the tests using:

```bash
dotnet test
```

## Git Hooks

The project includes Git hooks for code formatting:

- Pre-commit hook: Runs `dotnet format` to ensure consistent code style

To install Husky:

```bash
dotnet tool restore
```
