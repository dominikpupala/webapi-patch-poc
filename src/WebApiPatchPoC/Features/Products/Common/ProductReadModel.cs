namespace WebApiPatchPoC.Features.Products.Common;

internal sealed record ProductReadModel(
    string Sku,
    string Name,
    string ImgUri,
    decimal Price,
    string Description);
