using FluentValidation;
using WebApiPatchPoC.Data;
using WebApiPatchPoC.Features.Products.Common;
using WebApiPatchPoC.Features.Products.GetProductBySku;
using WebApiPatchPoC.Features.Products.GetProducts;

namespace WebApiPatchPoC;

internal static class ConfigureServices
{
    extension(WebApplicationBuilder builder)
    {
        public void AddServices()
        {
            builder.Services.AddOpenApi();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton, includeInternalTypes: true);

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
            builder.Services.AddScoped<GetProductBySkuHandler>();
            builder.Services.AddScoped<GetProductsHandler>();
        }
    }
}
