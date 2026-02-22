using Shared.Domain.Entities;
using Shared.Domain.Events;
using UserService.Domain.Enums;
using UserService.Domain.Events;
using UserService.Domain.Exceptions;
using UserService.Domain.ValueObjects;

namespace UserService.Domain.Entities;

public class UserEntity : GuidEntity
{
    public FullName FullName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public MobileNo MobileNo { get; private set; } = null!;
    public UserStatus Status { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsPhoneVerified { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private readonly List<UserRoleEntity> _roles = new();
    public IReadOnlyCollection<UserRoleEntity> Roles => _roles.AsReadOnly();

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private UserEntity() { }

    private void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public static UserEntity Create(
        string firstName,
        string lastName,
        string email,
        string phone)
    {
        var user = new UserEntity
        {
            FullName = new FullName(firstName, lastName),
            Email = new Email(email),
            MobileNo = new MobileNo(phone),
            Status = UserStatus.PendingVerification,
            IsEmailVerified = false,
            IsPhoneVerified = false
        };

        user.Raise(new UserCreatedEvent(user.Id, firstName, lastName, email, phone));
        return user;
    }

    public void VerifyEmail()
    {
        if (IsEmailVerified)
            throw new UserDomainException("Email is already verified");

        IsEmailVerified = true;

        if (IsPhoneVerified)
            Status = UserStatus.Active;

        Raise(new UserEmailVerifiedEvent(Id));
    }

    public void VerifyPhone()
    {
        if (IsPhoneVerified)
            throw new UserDomainException("Phone is already verified");

        IsPhoneVerified = true;

        if (IsEmailVerified)
            Status = UserStatus.Active;

        Raise(new UserPhoneVerifiedEvent(Id));
    }

    public void UpdateEmail(string newEmail)
    {
        if (Status == UserStatus.Suspended)
            throw new UserSuspendedException(Id);

        Email = new Email(newEmail);
        IsEmailVerified = false;
        Status = UserStatus.PendingVerification;

        Raise(new UserEmailUpdatedEvent(Id, newEmail));
    }

    public void UpdateMobileNo(string newMobileNo)
    {
        if (Status == UserStatus.Suspended)
            throw new UserSuspendedException(Id);

        MobileNo = new MobileNo(newMobileNo);
        IsPhoneVerified = false;

        Raise(new UserMobileNoUpdatedEvent(Id, newMobileNo));
    }

    public void UpdateName(string firstName, string lastName)
    {
        if (Status == UserStatus.Suspended)
            throw new UserSuspendedException(Id);

        FullName = new FullName(firstName, lastName);

        Raise(new UserNameUpdatedEvent(Id, firstName, lastName));
    }

    public void AssignRole(UserRoleType role)
    {
        if (_roles.Any(r => r.Role == role))
            throw new UserDomainException($"User already has role {role}");

        _roles.Add(UserRoleEntity.Create(Id, role));
        Raise(new UserRoleAssignedEvent(Id, role));
    }

    public void RemoveRole(UserRoleType role)
    {
        var userRole = _roles.FirstOrDefault(r => r.Role == role)
            ?? throw new UserDomainException($"User does not have role {role}");

        _roles.Remove(userRole);
        Raise(new UserRoleRemovedEvent(Id, role));
    }

    public void Suspend(string reason)
    {
        if (Status == UserStatus.Suspended)
            throw new UserDomainException("User is already suspended");

        Status = UserStatus.Suspended;
        Raise(new UserSuspendedEvent(Id, reason));
    }

    public void Activate()
    {
        if (Status == UserStatus.Active)
            throw new UserDomainException("User is already active");

        if (!IsEmailVerified || !IsPhoneVerified)
            throw new UserDomainException("User must verify email and phone first");

        Status = UserStatus.Active;
        Raise(new UserActivatedEvent(Id));
    }

    public void RecordLogin()
        => LastLoginAt = DateTime.UtcNow;

    public bool HasRole(UserRoleType role)
        => _roles.Any(r => r.Role == role);

    public bool IsActive()
        => Status == UserStatus.Active;
}