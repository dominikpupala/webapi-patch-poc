using FluentValidation;
using WebApiPatchPoC.Filters;

namespace WebApiPatchPoC.Features.Products.GetProductBySku;

internal static class GetProductBySkuEndpoint
{
    public sealed record Request(string Sku);

    public sealed record Response(
        string Sku,
        string Name,
        string ImgUri,
        decimal Price,
        string Description);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Sku)
                .NotEmpty()
                .WithMessage("SKU is required")

                .MaximumLength(50)
                .WithMessage("SKU cannot exceed 50 characters")

                .Must(sku => !string.IsNullOrWhiteSpace(sku))
                .WithMessage("SKU cannot be empty or whitespace")

                .Matches(@"^[\x20-\x7E]*$")
                .WithMessage("SKU can only contain ASCII printable characters")

                .Must(sku => sku.Trim() == sku)
                .WithMessage("SKU cannot have leading or trailing whitespace");
        }
    }

    extension(IEndpointRouteBuilder group)
    {
        public void MapGetProductBySku()
            => group
            .MapGet("/{sku}", static async ([AsParameters] Request request, GetProductBySkuHandler handler) =>
            {
                var query = new GetProductBySkuHandler.Query(request.Sku);
                var result = await handler.Handle(query);

                return result.Match(
                    product => Results.Ok(new Response(
                        product.Sku,
                        product.Name,
                        product.ImgUri,
                        product.Price,
                        product.Description)),
                    notFound => Results.NotFound());
            })
            .WithName("GetProductBySku")
            .WithSummary("Get a product by SKU")
            .WithDescription("Retrieves a single product from the catalog by its SKU")
            .Produces<Response>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithRequestValidation<Request>();
    }
}
