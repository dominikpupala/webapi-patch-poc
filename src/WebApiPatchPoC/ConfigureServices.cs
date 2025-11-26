namespace WebApiPatchPoC;

internal static class ConfigureServices
{
    extension(WebApplicationBuilder builder)
    {
        public void AddServices()
        {
            builder.Services.AddOpenApi();
        }
    }
}
