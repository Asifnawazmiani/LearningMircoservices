using Shared.Domain.UnitOfWork;
using UserService.Contracts.Requests;
using UserService.Contracts.Responses;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using UserService.Domain.Services;

namespace UserService.Infrastructure.Services;

public class UserService(IUserRepository userRepository, IUnitOfWork unitOfWork) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        // Service creates domain entity (not controller!)
        var user = UserEntity.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.MobileNo
        );

        await _userRepository.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Map to Response
        return MapToResponse(user);
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"User with id '{id}' was not found.");

        _userRepository.Delete(user);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<List<UserResponse>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await _userRepository.GetAllAsync(ct);
        return users.Select(MapToResponse).ToList();
    }

    public async Task<UserResponse> GetUserByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"User with id '{id}' was not found.");

        return MapToResponse(user);
    }

    public async Task<UserResponse> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"User with id '{id}' was not found.");

        // Update domain entity using its methods
        user.UpdateName(request.FirstName, request.LastName);
        user.UpdateEmail(request.Email);
        user.UpdateMobileNo(request.MobileNo);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return MapToResponse(user);
    }

    // Private helper to map entity to Response
    private static UserResponse MapToResponse(UserEntity entity)
    {
        return new UserResponse
        {
            Id = entity.Id,
            FirstName = entity.FullName.FirstName,
            LastName = entity.FullName.LastName,
            FullName = $"{entity.FullName.FirstName} {entity.FullName.LastName}",
            Email = entity.Email.Value,
            MobileNo = entity.MobileNo.Value,
            Status = entity.Status.ToString(),
            IsEmailVerified = entity.IsEmailVerified,
            IsPhoneVerified = entity.IsPhoneVerified,
            LastLoginAt = entity.LastLoginAt,
            Roles = entity.Roles.Select(r => r.Role.ToString()).ToList(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
