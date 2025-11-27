namespace WebApiPatchPoC.Features.Products.Domain;

// Domain types following DDD functional style
internal sealed record Product(
    string Sku,
    string Name,
    string ImgUri,
    decimal Price,
    string? Description);
