using WebApiPatchPoC.Common;
using WebApiPatchPoC.Features.Products.Common;

namespace WebApiPatchPoC.Features.Products.GetProductsPaginated;

internal sealed class GetProductsPaginatedHandler(IProductReadService productReadService)
{
    public sealed record Query(int PageNumber, int PageSize);

    public async Task<PaginatedResult<ProductReadModel>> Handle(Query query)
        => await productReadService.GetProductsPaginated(query.PageNumber, query.PageSize);
}
