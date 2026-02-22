using UserService.Contracts.Requests;
using UserService.Contracts.Responses;

namespace UserService.Domain.Services;

/// <summary>
/// Domain service interface for user operations.
/// Uses Request/Response pattern from UserService.Contracts.
/// </summary>
public interface IUserService
{
    Task<UserResponse> GetUserByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<UserResponse>> GetAllUsersAsync(CancellationToken ct = default);
    Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<UserResponse> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default);
    Task DeleteUserAsync(Guid id, CancellationToken ct = default);
}
