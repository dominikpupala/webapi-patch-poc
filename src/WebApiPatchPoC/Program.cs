using WebApiPatchPoC;
using WebApiPatchPoC.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();

var app = builder.Build();

// Initialize database in development
if (app.Environment.IsDevelopment())
{
    var connectionString = app.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    await DatabaseSeeder.Initialize(connectionString, logger);
}

app.Configure();
await app.RunAsync();
