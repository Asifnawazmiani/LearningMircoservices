using UserService.Domain.Enums;

namespace UserService.Domain.Events;

public record UserCreatedEvent(Guid UserId, string FirstName, string LastName, string Email, string MobileNo) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserEmailVerifiedEvent(Guid UserId) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserPhoneVerifiedEvent(Guid UserId) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserEmailUpdatedEvent(Guid UserId, string NewEmail) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserMobileNoUpdatedEvent(Guid UserId, string NewMobileNo) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserNameUpdatedEvent(Guid UserId, string FirstName, string LastName) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserSuspendedEvent(Guid UserId, string Reason) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserActivatedEvent(Guid UserId) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserRoleAssignedEvent(Guid UserId, UserRoleType Role) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserRoleRemovedEvent(Guid UserId, UserRoleType Role) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
