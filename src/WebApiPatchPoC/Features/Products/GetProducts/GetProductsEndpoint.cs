namespace WebApiPatchPoC.Features.Products.GetProducts;

internal static class GetProductsEndpoint
{
    public sealed record Response(
        string Sku,
        string Name,
        string ImgUri,
        decimal Price,
        string Description);

    extension(IEndpointRouteBuilder group)
    {
        public void MapGetProducts()
            => group
            .MapGet("/", static async (GetProductsHandler handler) =>
            {
                var products = await handler.Handle();
                var response = products.Select(p => new Response(
                    p.Sku,
                    p.Name,
                    p.ImgUri,
                    p.Price,
                    p.Description));

                return Results.Ok(response);
            })
            .WithName("GetProducts")
            .WithSummary("Get all products")
            .WithDescription("Retrieves a list of all available products from the catalog")
            .Produces<IEnumerable<Response>>(StatusCodes.Status200OK);
    }
}
