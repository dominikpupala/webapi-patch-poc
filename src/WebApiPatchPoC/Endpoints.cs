using WebApiPatchPoC.Features.Products.GetProducts;

namespace WebApiPatchPoC;

internal static class Endpoints
{
    extension(WebApplication app)
    {
        public void MapEndpoints()
        {
            var endpoints = app.MapGroup("api/");

            endpoints.MapProductsEndpoints();
        }
    }

    extension(IEndpointRouteBuilder endpoints)
    {
        public void MapProductsEndpoints()
        {
            var productsGroup = endpoints
                .MapGroup("products")
                .WithTags("Products");

            productsGroup.MapGetProducts();
        }
    }
}
