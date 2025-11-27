using WebApiPatchPoC.Common;

namespace WebApiPatchPoC.Features.Products.Common;

internal interface IProductReadService
{
    Task<IReadOnlyList<ProductReadModel>> GetProducts();
    Task<ProductReadModel?> GetProductBySku(string sku);
    Task<PaginatedResult<ProductReadModel>> GetProductsPaginated(int pageNumber, int pageSize);
}
