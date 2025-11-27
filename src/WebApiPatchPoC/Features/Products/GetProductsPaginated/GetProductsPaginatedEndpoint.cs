using FluentValidation;
using WebApiPatchPoC.Filters;

namespace WebApiPatchPoC.Features.Products.GetProductsPaginated;

internal static class GetProductsPaginatedEndpoint
{
    public sealed record Request(int PageNumber = 1, int PageSize = 10);

    public sealed record Response(
        string Sku,
        string Name,
        string ImgUri,
        decimal Price,
        string Description);

    internal sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100");
        }
    }

    public sealed record PaginatedResponse(
        IEnumerable<Response> Items,
        int TotalCount,
        int PageNumber,
        int PageSize,
        int TotalPages);

    extension(IEndpointRouteBuilder group)
    {
        public void MapGetProductsPaginated()
            => group
            .MapGet("/", static async ([AsParameters] Request request, GetProductsPaginatedHandler handler) =>
            {
                var query = new GetProductsPaginatedHandler.Query(request.PageNumber, request.PageSize);
                var result = await handler.Handle(query);

                var response = new PaginatedResponse(
                    result.Items.Select(p => new Response(
                        p.Sku,
                        p.Name,
                        p.ImgUri,
                        p.Price,
                        p.Description)),
                    result.TotalCount,
                    result.PageNumber,
                    result.PageSize,
                    (int)Math.Ceiling((double)result.TotalCount / result.PageSize));

                return Results.Ok(response);
            })
            .WithName("GetProductsPaginated")
            .WithSummary("Get products with pagination")
            .WithDescription("Retrieves a paginated list of products from the catalog. Supports page number and page size query parameters.")
            .Produces<PaginatedResponse>(StatusCodes.Status200OK)
            .WithRequestValidation<Request>()
            .MapToApiVersion(2);
    }
}
