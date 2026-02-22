using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InventoryService.Infrastructure;

/// <summary>
/// Design-time factory for InventoryDbContext to support EF Core migrations
/// </summary>
public class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
        
        // Use a temporary connection string for migrations
        // This won't be used at runtime - Aspire provides the real connection string
        optionsBuilder.UseNpgsql("Host=localhost;Database=inventory;Username=postgres;Password=postgres");

        return new InventoryDbContext(optionsBuilder.Options);
    }
}
