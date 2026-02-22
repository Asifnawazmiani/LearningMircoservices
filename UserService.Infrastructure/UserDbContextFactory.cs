using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UserService.Infrastructure;

/// <summary>
/// Design-time factory for UserDbContext to support EF Core migrations
/// </summary>
public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
        
        // Use a temporary connection string for migrations
        // This won't be used at runtime - Aspire provides the real connection string
        optionsBuilder.UseNpgsql("Host=localhost;Database=users;Username=postgres;Password=postgres");

        return new UserDbContext(optionsBuilder.Options);
    }
}
