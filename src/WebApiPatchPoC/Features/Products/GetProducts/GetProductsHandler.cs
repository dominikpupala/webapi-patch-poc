using WebApiPatchPoC.Features.Products.Common;

namespace WebApiPatchPoC.Features.Products.GetProducts;

internal sealed class GetProductsHandler(IProductReadService productReadService)
{
    public async Task<IReadOnlyList<ProductReadModel>> Handle()
        => await productReadService.GetProducts();
}
