# Weather Forecast App

Modern full-stack sample that pairs an ASP.NET Core Web API with a React (Vite) frontend. The project demonstrates a layered backend with Domain Events, repository + unit-of-work patterns, DryIoc-based DI, Entity Framework Core (SQL Server), and validated CRUD UI backed by Azure-hosted infrastructure.

## Stack

- **Backend:** ASP.NET Core 8 Web API, DryIoc DI, Entity Framework Core (SQL Server/InMemory/Sqlite providers)
- **Frontend:** React 19 + TypeScript, Vite, react-hook-form + Zod validation
- **Testing:** xUnit unit tests (mocked EF access) and integration tests using `WebApplicationFactory`
- **Cloud:** Azure App Service (API), Azure SQL Database, Azure Static Web Apps (SPA)

## Repository Layout

```
Server/                  # ASP.NET Core solution
  Domain/                # Entities, domain events, abstractions
  Application/           # Services, DTOs, event handlers
  Infrastructure/        # EF Core DbContext, repositories, UoW, event dispatcher
  Controllers/           # Weather forecast CRUD endpoints
  Migrations/            # EF Core migrations
client/                  # React + Vite frontend
  src/                   # Components, hooks, API client
  public/                # Static assets
Tests/                   # xUnit unit + integration tests
docs/                    # Deployment notes, additional documentation
```

## Local Development

### Prerequisites

- .NET 8 SDK
- Node.js 20+
- SQL Server (LocalDB, Azure SQL, or connection string for your instance)

### Configure connection string

Use [dotnet user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) to keep credentials out of source control:

```bash
cd Server
dotnet user-secrets init
DOTNET_ENVIRONMENT=Development \
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=tcp:localhost,1433;Database=WeatherForecastDb;User ID=...;Password=...;Encrypt=True;TrustServerCertificate=True;"
```

### Database migrations

```bash
cd ..
DOTNET_ENVIRONMENT=Development dotnet ef database update --project Server --startup-project Server
```

### Run the backend

```bash
dotnet run --project Server
```

API swagger UI is available at `https://localhost:5001/swagger` (development environment only).

### Run the frontend

```bash
cd client
npm install
npm run dev
```

Visit `http://localhost:5173` to interact with the dashboard.

## Testing

```bash
dotnet test             # Executes unit + integration tests
npm run build           # Ensures frontend builds without type/lint errors
```

Integration tests use the in-memory provider; no external services are required.

## Azure Deployment

1. **Backend** – deploy `Server` to Azure App Service (`dotnet publish` + `az webapp deploy` or GitHub Actions):
   - Set `ConnectionStrings:DefaultConnection` (type `SQLAzure`) in App Service Configuration.
   - Configure `Frontend__AllowedOrigins` with the Static Web App URL for CORS.
2. **Database** – Azure SQL Database with `WeatherForecastDb`; migrations run automatically on startup via `ApplyMigrationsAsync`.
3. **Frontend** – Azure Static Web App tied to this GitHub repo (`client` as app location, `dist` output). Set `VITE_API_URL` app setting to the public API base URL.
4. Refer to [`docs/azure-deployment.md`](docs/azure-deployment.md) for step-by-step guidance.

## CI/CD

- Azure Static Web Apps GitHub Action builds and deploys frontend from `main`.
- Extend with additional workflows for backend (GitHub Actions + `azure/webapps-deploy`) or testing as needed.

## Environment Variables

| Setting                         | Description                                             |
|---------------------------------|---------------------------------------------------------|
| `ConnectionStrings:DefaultConnection` | SQL Server connection used by EF Core                |
| `Frontend__AllowedOrigins`      | Comma-separated list of allowed CORS origins            |
| `Database:Provider`             | Optional override (`SqlServer`/`Sqlite`/`InMemory`)     |
| `Database:InMemoryName`         | In-memory database name for tests                       |
| `VITE_API_URL`                  | Frontend base URL for API (Static Web App configuration)|

## Licensing

Add an appropriate license for your usage (e.g., MIT) and update this section accordingly.
