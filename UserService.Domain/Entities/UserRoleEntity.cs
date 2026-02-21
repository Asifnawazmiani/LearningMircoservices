using UserService.Domain.Enums;

namespace UserService.Domain.Entities;

public class UserRoleEntity
{
    public Guid UserId { get; private set; }
    public UserRoleType Role { get; private set; }

    public UserEntity User { get; private set; } = null!;

    private UserRoleEntity() { }

    public static UserRoleEntity Create(Guid userId, UserRoleType role)
    {
        return new UserRoleEntity
        {
            UserId = userId,
            Role = role
        };
    }
}
