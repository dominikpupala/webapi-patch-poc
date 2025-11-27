namespace WebApiPatchPoC;

internal static class ConfigureApp
{
    extension(WebApplication app)
    {
        public void Configure()
        {
            app.UseVersionedApi();
            app.MapEndpoints();
        }

        public WebApplication UseVersionedApi()
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                foreach (var version in ApiVersions.Versions.Values)
                {
                    var docName = version.ToDocumentName;
                    options.SwaggerEndpoint($"/openapi/{docName}.json", docName.ToUpperInvariant());
                }
            });

            return app;
        }
    }
}
