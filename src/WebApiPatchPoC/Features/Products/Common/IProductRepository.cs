using WebApiPatchPoC.Common.Events;
using WebApiPatchPoC.Features.Products.Domain;

namespace WebApiPatchPoC.Features.Products.Common;

/// <summary>
/// Repository for the Product aggregate root
/// </summary>
internal interface IProductRepository
{
    /// <summary>
    /// Loads a product by SKU from the database and reconstructs the domain entity
    /// </summary>
    Task<Product?> GetBySku(string sku);

    /// <summary>
    /// Saves the product domain entity to the database
    /// </summary>
    Task<bool> Save(Product product);
}
