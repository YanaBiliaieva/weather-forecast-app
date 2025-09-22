# Azure Deployment Notes

This solution is ready to run as two apps: an ASP.NET Core Web API (`Server`) and a React single-page application (`client`). The recommended approach is to host them separately and wire them up with a shared API base URL.

## 1. Publish the API to Azure App Service

1. Build a release publish profile:
   ```bash
   dotnet publish Server/Server.csproj -c Release -o publish
   ```
2. From Azure Portal create a new **App Service** (Windows or Linux) targeting **.NET 8** runtime.
3. Configure the connection string named `DefaultConnection` in the App Service settings to point to your Azure SQL database.
4. Set the `Frontend__AllowedOrigins` setting (double underscore for nesting) to the domain that will host the React frontend.
5. Deploy the published artifacts using `az webapp deploy`, GitHub Actions, or the built-in Visual Studio publish flow.
6. On first startup the app runs `ApplyMigrationsAsync`, automatically creating/updating the database schema.

## 2. Provision Azure SQL Database

1. Create an Azure SQL logical server and a database (e.g., `WeatherForecastDb`).
2. Allow Azure services and your local IP in the firewall settings.
3. Update the App Service connection string to use the SQL server address, user, and password. Example:
   `Server=tcp:<server>.database.windows.net,1433;Database=WeatherForecastDb;User ID=<user>;Password=<password>;Encrypt=True;`

## 3. Deploy the React frontend

1. Build the production bundle:
   ```bash
   cd client
   npm run build
   ```
2. Host the contents of `client/dist` on Azure Static Web Apps or Azure Storage static website.
3. Configure the static site to proxy API requests to the Web API host, e.g. `/api/*` → `https://<api-app>.azurewebsites.net/api/*`.
4. Set `VITE_API_URL` environment variable for the static site to the API base (`https://<api-app>.azurewebsites.net/api`).

## 4. Continuous integration (optional)

- Backend: GitHub Actions workflow using `dotnet restore/build/test/publish` followed by `azure/webapps-deploy` action.
- Frontend: GitHub Actions or Azure Static Web Apps workflow running `npm ci && npm run build`.

## 5. Observability and scaling

- Enable Application Insights on the App Service to collect telemetry.
- Configure auto-scale rules or manual scale-up depending on load.
- Use Azure Monitor alerts on failure counts or response times.

With these steps you can operate the API and SPA independently while keeping the infrastructure simple.
