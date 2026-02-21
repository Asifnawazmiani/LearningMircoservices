namespace Infrastructure.Persistence.Entities;

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    // Called by BaseDbContext automatically
    internal void SetUpdatedAt(DateTime updatedAt)
        => UpdatedAt = updatedAt;

    internal void SetDeleted(DateTime deletedAt)
    {
        IsDeleted = true;
        DeletedAt = deletedAt;
    }
}

// Convenient aliases — services choose their ID type
public abstract class GuidEntity : BaseEntity<Guid>
{
    protected GuidEntity()
    {
        Id = Guid.CreateVersion7();
    }
}

public abstract class LongEntity : BaseEntity<long>
{
    // Id assigned by database
}
