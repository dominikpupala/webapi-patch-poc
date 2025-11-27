using WebApiPatchPoC.Data;
using WebApiPatchPoC.Features.Products.Common;

namespace WebApiPatchPoC;

internal static class ConfigureServices
{
    extension(WebApplicationBuilder builder)
    {
        public void AddServices()
        {
            builder.Services.AddOpenApi();

            builder.AddDatabase();
            builder.AddFeatures();
        }

        private void AddDatabase()
        {
            builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        }

        private void AddFeatures()
        {
            // products
            builder.Services.AddScoped<IProductReadService, ProductReadService>();
        }
    }
}
