# WebApiPatchPoC

A small proof-of-concept Web API demonstrating PATCH endpoints with validation and partial updates in .NET.

## Current State

- Basic project setup with OpenAPI/SwaggerUI integration
- Extension methods for service and app configuration
- Automatic initialization and seeding for MS SQL database
	- 50 sample products seeded on startup

## Features

### Products API
- **GET** `/api/v1/products` - Retrieve all products
- **GET** `/api/v2/products` - Retrieve all products paginated
- **GET** `/api/v1/products/{sku}` - Retrieve a product by SKU
- **PATCH** `/api/v1/products/{sku}` - Partially update a product using JSON Merge Patch (RFC 7396)

## Requirements

- Visual Studio 2026 or later
- .NET 10 SDK
- SQL Server instance (use your own or Docker Compose provided)

## Running the Application

1. Ensure you have a SQL Server instance running
2. Update the connection string in `appsettings.Development.json`
3. Run the application from Visual Studio
4. Database will be created and seeded automatically on first run
5. Browse to `/swagger` for API documentation

See [DATABASE_SETUP.md](DATABASE_SETUP.md) for detailed database setup instructions

## Running the Tests

### Command Line
```bash
# Run all tests
dotnet test
```

### Visual Studio
- Open **Test Explorer** (Test > Test Explorer)
- Click "Run All Tests" or right-click individual tests to run them
