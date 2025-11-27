namespace WebApiPatchPoC.Common.Events;

// Domain events
internal interface IDomainEvent
{
    DateTime OccurredAt { get; }
}
