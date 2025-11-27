using Asp.Versioning;
using FluentValidation;
using WebApiPatchPoC.Data;
using WebApiPatchPoC.Features.Products.Common;
using WebApiPatchPoC.Features.Products.GetProductBySku;
using WebApiPatchPoC.Features.Products.GetProducts;
using WebApiPatchPoC.Features.Products.GetProductsPaginated;

namespace WebApiPatchPoC;

internal static class ConfigureServices
{
    extension(WebApplicationBuilder builder)
    {
        public void AddServices()
        {
            builder.Services.AddVersionedApiServices();
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
            builder.Services.AddScoped<GetProductsPaginatedHandler>();
        }
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection AddVersionedApiServices()
        {
            services
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = ApiVersions.DefaultVersion;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                    options.AssumeDefaultVersionWhenUnspecified = true;
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'V";
                    options.SubstituteApiVersionInUrl = true;
                });

            foreach (var version in ApiVersions.Versions.Values)
            {
                services.AddOpenApi(version.ToDocumentName);
            }

            return services;
        }
    }
}
