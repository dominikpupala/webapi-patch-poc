using WebApiPatchPoC.Common.Events;
using WebApiPatchPoC.Features.Products.Domain.Events;

namespace WebApiPatchPoC.Features.Products.Domain.Workflows;

internal static class UpdateDescriptionWorkflow
{
    public static (Product Product, List<IDomainEvent> Events) ChangeDescription(Product product, string? newDescription)
    {
        if (newDescription == product.Description)
        {
            return (product, []);
        }

        var updatedProduct = product with { Description = newDescription };

        var events = new List<IDomainEvent>
        {
            new ProductDescriptionUpdated(
                Sku: product.Sku,
                OldDescription: product.Description,
                NewDescription: newDescription,
                OccurredAt: DateTime.UtcNow)
        };

        return (updatedProduct, events);
    }
}
