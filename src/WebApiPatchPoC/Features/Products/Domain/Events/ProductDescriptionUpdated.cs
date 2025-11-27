using WebApiPatchPoC.Common.Events;

namespace WebApiPatchPoC.Features.Products.Domain.Events;

internal sealed record ProductDescriptionUpdated(
    string Sku,
    string? OldDescription,
    string? NewDescription,
    DateTime OccurredAt) : IDomainEvent;
