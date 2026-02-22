using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderService.Infrastructure;

/// <summary>
/// Design-time factory for OrderDbContext to support EF Core migrations
/// </summary>
public class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
{
    public OrderDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
        
        // Use a temporary connection string for migrations
        // This won't be used at runtime - Aspire provides the real connection string
        optionsBuilder.UseNpgsql("Host=localhost;Database=orders;Username=postgres;Password=postgres");

        return new OrderDbContext(optionsBuilder.Options);
    }
}
