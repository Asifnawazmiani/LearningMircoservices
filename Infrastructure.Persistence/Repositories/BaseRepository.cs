using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

// src/Infrastructure/Infrastructure.Persistence/
// Repositories/BaseRepository.cs

public abstract class BaseRepository<TEntity, TId>(DbContext dbContext) : IBaseRepository<TEntity, TId> where TEntity : BaseEntity<TId>
{
    protected readonly DbContext _dbContext = dbContext;
    protected readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default)
    {
        return await _dbSet.FindAsync([id], ct);
    }

    public async Task<List<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbSet.ToListAsync(ct);
    }

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity); // soft delete handled by BaseDbContext
    }

    public async Task<bool> ExistsAsync(TId id, CancellationToken ct = default)
    {
        return await _dbSet.AnyAsync(e => EF.Property<TId>(e, "Id")!.Equals(id), ct);
    }
}