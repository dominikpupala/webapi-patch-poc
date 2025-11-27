namespace WebApiPatchPoC.Features.Products.Common;

internal interface IProductReadService
{
    Task<IReadOnlyList<ProductReadModel>> GetProducts();
}
