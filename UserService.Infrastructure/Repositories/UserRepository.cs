using Infrastructure.Persistence.Repositories;
using UserService.Domain.Enums;
using UserService.Domain.Repositories;

namespace UserService.Infrastructure.Repositories;

public class UserRepository(UserDbContext dbContext) : BaseRepository<UserEntity, Guid>(dbContext), IUserRepository
{
    public Task<UserEntity?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.ToLowerInvariant().Trim();
        return _dbSet.FirstOrDefaultAsync(u => u.Email.Value == normalized, ct);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.ToLowerInvariant().Trim();
        return _dbSet.AnyAsync(u => u.Email.Value == normalized, ct);
    }

    public Task<List<UserEntity>> GetByStatusAsync(UserStatus status, CancellationToken ct = default)
    {
        return _dbSet
            .Where(u => u.Status == status)
            .ToListAsync(ct);
    }
}

