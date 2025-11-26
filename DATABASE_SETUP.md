# Database Setup

## Requirements

This application requires a **Microsoft SQL Server** instance. The server can be empty - the application will automatically create the database, initialize the schema, and seed data when running in **Development** environment.

## Automatic Database Initialization

The application includes a `DatabaseSeeder` that runs automatically in Development mode when:
- Running with `ASPNETCORE_ENVIRONMENT=Development` environment variable, **OR**
- Running from Visual Studio (which sets this variable via `launchSettings.json`)

The seeder will:
1. Create the database specified in the connection string (if it doesn't exist)
2. Create the `Products` table schema
3. Seed 20 sample products

## Option 1: Using Docker Compose (Recommended)

Start SQL Server in a Docker container:

```bash
docker compose up -d
```

This will start SQL Server with:
- **Server**: `localhost,1433` (when running from WSL2)
- **Username**: `sa`
- **Password**: `Devtest123!`
- **Database**: Empty server (database will be created automatically)

### Getting the WSL2 IP address

If you're running the application from Windows (or need to connect from SSMS), get the WSL2 IP:

```bash
ip addr show eth0 | grep "inet " | awk '{print $2}' | cut -d/ -f1
```

Then update the connection string in `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<WSL2_IP>,1433;Database=ProductsDb;User Id=sa;Password=Devtest123!;TrustServerCertificate=True;Encrypt=True;"
  }
}
```

### Docker Compose Commands

```bash
# Stop container (keeps data)
docker compose stop

# Remove container (keeps data in volume)
docker compose down

# Remove everything including data
docker compose down -v
```

## Option 2: Using Existing SQL Server

If you have an existing SQL Server instance, update the connection string in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ProductsDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;Encrypt=True;"
  }
}
```

> **Note:** The connection string is stored in `appsettings.Development.json` for convenience in this demo. In production, use secure secret management (User Secrets for local dev, Azure Key Vault/AWS Secrets Manager for production).
