using UserService.Domain.Entities;

namespace UserService.Domain.Services;

public interface IUserService
{
        Task<UserEntity> GetUserByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<UserEntity>> GetAllUsersAsync(CancellationToken ct = default);
        Task CreateUserAsync(UserEntity request, CancellationToken ct = default);
        Task UpdateUserAsync(Guid id, UserEntity request, CancellationToken ct = default);
        Task DeleteUserAsync(Guid id, CancellationToken ct = default);
}
