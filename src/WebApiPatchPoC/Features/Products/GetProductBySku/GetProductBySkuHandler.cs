using OneOf;
using OneOf.Types;
using WebApiPatchPoC.Features.Products.Common;

namespace WebApiPatchPoC.Features.Products.GetProductBySku;

internal sealed class GetProductBySkuHandler(IProductReadService productReadService)
{
    public sealed record Query(string Sku);

    public async Task<OneOf<ProductReadModel, NotFound>> Handle(Query query)
    {
        var product = await productReadService.GetProductBySku(query.Sku);
        return product is not null ? product : new NotFound();
    }
}
