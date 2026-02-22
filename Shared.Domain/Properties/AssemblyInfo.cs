using System.Runtime.CompilerServices;

// Allow Infrastructure.Persistence to access internal members
// This is necessary for BaseDbContext to call SetUpdatedAt() and SetDeleted()
// on BaseEntity instances during SaveChanges lifecycle management

// ═══════════════════════════════════════════════════════════════
// THIS LINE IS THE KEY! Without it, Infrastructure.Persistence
// cannot call internal methods like SetUpdatedAt() and SetDeleted()
// ═══════════════════════════════════════════════════════════════
[assembly: InternalsVisibleTo("Infrastructure.Persistence")]
