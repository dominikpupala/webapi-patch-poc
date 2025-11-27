using Asp.Versioning.Builder;
using WebApiPatchPoC.Features.Products.GetProductBySku;
using WebApiPatchPoC.Features.Products.GetProducts;
using WebApiPatchPoC.Features.Products.GetProductsPaginated;

namespace WebApiPatchPoC;

internal static class Endpoints
{
    extension(WebApplication app)
    {
        public void MapEndpoints()
        {
            var versionSet = app.CreateVersionSet();

            var endpoints = app
                .MapGroup("api/v{version:apiVersion}")
                .WithApiVersionSet(versionSet);

            endpoints.MapProductsEndpoints();
        }

        public ApiVersionSet CreateVersionSet()
        {
            var versionSetBuilder = app.NewApiVersionSet();

            foreach (var version in ApiVersions.Versions.Values)
            {
                versionSetBuilder.HasApiVersion(version);
            }

            return versionSetBuilder
                .ReportApiVersions()
                .Build();
        }
    }

    extension(IEndpointRouteBuilder endpoints)
    {
        public void MapProductsEndpoints()
        {
            var productsGroup = endpoints
                .MapGroup("products")
                .WithTags("Products");

            productsGroup.MapGetProductBySku();
            productsGroup.MapGetProducts();
            productsGroup.MapGetProductsPaginated();
        }
    }
}
