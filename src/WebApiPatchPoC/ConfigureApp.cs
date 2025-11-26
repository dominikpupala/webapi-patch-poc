namespace WebApiPatchPoC;

internal static class ConfigureApp
{
    extension(WebApplication app)
    {
        public void Configure()
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
            app.MapEndpoints();
        }
    }
}
