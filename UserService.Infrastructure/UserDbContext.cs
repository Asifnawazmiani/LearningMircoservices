namespace UserService.Infrastructure;

public class UserDbContext(DbContextOptions<UserDbContext> options) : BaseDbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        WriteOutboxMessages();
        return await base.SaveChangesAsync(ct);
    }

    public override int SaveChanges()
    {
        WriteOutboxMessages();
        return base.SaveChanges();
    }

    private void WriteOutboxMessages()
    {
        var domainEvents = ChangeTracker
            .Entries<UserEntity>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        if (domainEvents.Count == 0)
            return;

        foreach (var domainEvent in domainEvents)
        {
            OutboxMessages.Add(new OutboxMessage
            {
                EventType = domainEvent.GetType().FullName!,
                Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
            });
        }

        foreach (var entry in ChangeTracker.Entries<UserEntity>())
            entry.Entity.ClearDomainEvents();
    }
}


