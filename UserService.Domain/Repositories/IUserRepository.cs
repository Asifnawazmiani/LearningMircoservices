using Infrastructure.Persistence.Repositories;
using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Domain.Repositories;

public interface IUserRepository : IBaseRepository<UserEntity, Guid>
{
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<List<UserEntity>> GetByStatusAsync(UserStatus status, CancellationToken ct = default);
}
