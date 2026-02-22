namespace Shared.Domain.Events;

/// <summary>
/// Base interface for all domain events across microservices.
/// Domain events represent something that happened in the domain that domain experts care about.
/// </summary>
public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredAt { get; }
}
