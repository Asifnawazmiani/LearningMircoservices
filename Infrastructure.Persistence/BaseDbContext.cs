// src/Infrastructure/Infrastructure.Persistence/BaseDbContext.cs
using Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;
using Shared.Domain.UnitOfWork;
using System.Linq.Expressions;

namespace Infrastructure.Persistence;

public abstract class BaseDbContext : DbContext, IUnitOfWork
{
    protected BaseDbContext(DbContextOptions options) : base(options) { }

    // Every service gets outbox table
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure OutboxMessage
        ConfigureOutboxMessage(modelBuilder);

        // Auto apply soft delete filter
        // to all entities that extend BaseEntity
        ApplySoftDeleteFilter(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        HandleAuditableEntities();
        return await base.SaveChangesAsync(ct);
    }

    public override int SaveChanges()
    {
        HandleAuditableEntities();
        return base.SaveChanges();
    }

    // ═══════════════════════════════════════
    // Private Methods
    // ═══════════════════════════════════════

    private void HandleAuditableEntities()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            // Only handle BaseEntity<TId> derived entities
            var entityType = entry.Entity.GetType();

            var isBaseEntity = entityType.BaseType is not null &&
                entityType.BaseType.IsGenericType &&
                entityType.BaseType.GetGenericTypeDefinition() == typeof(BaseEntity<>);

            var isGuidEntity = entry.Entity is GuidEntity;
            var isLongEntity = entry.Entity is LongEntity;

            if (!isGuidEntity && !isLongEntity)
                continue;

            switch (entry.State)
            {
                case EntityState.Modified:
                    // Auto set UpdatedAt
                    if (entry.Entity is GuidEntity guidEntity)
                        guidEntity.SetUpdatedAt(now);
                    else if (entry.Entity is LongEntity longEntity)
                        longEntity.SetUpdatedAt(now);
                    break;

                case EntityState.Deleted:
                    // Convert hard delete to soft delete
                    entry.State = EntityState.Modified;

                    if (entry.Entity is GuidEntity softGuid)
                        softGuid.SetDeleted(now);
                    else if (entry.Entity is LongEntity softLong)
                        softLong.SetDeleted(now);
                    break;
            }
        }
    }

    private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Check if entity extends GuidEntity or LongEntity
            var clrType = entityType.ClrType;

            var isGuidEntity = typeof(GuidEntity)
                .IsAssignableFrom(clrType);

            var isLongEntity = typeof(LongEntity)
                .IsAssignableFrom(clrType);

            if (!isGuidEntity && !isLongEntity)
                continue;

            // Add global query filter
            // SELECT * FROM table WHERE IsDeleted = false
            var parameter = Expression
                .Parameter(clrType, "e");

            var property = Expression
                .Property(parameter, nameof(GuidEntity.IsDeleted));

            var filter = Expression
                .Lambda(Expression.Not(property), parameter);

            modelBuilder
                .Entity(clrType)
                .HasQueryFilter(filter);
        }
    }

    private static void ConfigureOutboxMessage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.EventType)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Payload)
                .IsRequired();

            entity.Property(e => e.LastError)
                .HasMaxLength(2000);

            // Indexes for worker queries
            entity.HasIndex(e => e.IsProcessed);
            entity.HasIndex(e => e.IsPublished);
            entity.HasIndex(e => e.IsDead);
            entity.HasIndex(e => e.CreatedAt);

            // Composite index for worker main query
            entity.HasIndex(e => new
            {
                e.IsPublished,
                e.IsProcessed,
                e.IsDead,
                e.RetryCount
            });
        });
    }
}