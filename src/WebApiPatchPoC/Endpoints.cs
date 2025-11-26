namespace WebApiPatchPoC;

internal static class Endpoints
{
    extension(WebApplication app)
    {
        public void MapEndpoints()
        {
            var endpoints = app.MapGroup("api/");

            endpoints.MapTemporaryEndpoints();
        }
    }

    extension(IEndpointRouteBuilder endpoints)
    {
        public void MapTemporaryEndpoints()
        {
            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            endpoints.MapGet("/weatherforecast", () =>
            {
#pragma warning disable CA5394 // Do not use insecure randomness
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                    .ToArray();
#pragma warning restore CA5394 // Do not use insecure randomness
                return forecast;
            })
            .WithName("GetWeatherForecast");
        }
    }
}

#pragma warning disable CA1852// Temporary code
internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
#pragma warning restore CA1852
