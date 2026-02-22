using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UserService.Infrastructure.DataSeeders;

public static class DatabaseSeederExtensions
{
    /// <summary>
    /// Seeds the database with initial data. Should be called after migrations.
    /// </summary>
    public static async Task SeedUserDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<UserDataSeeder>>();

        var seeder = new UserDataSeeder(context, logger);
        await seeder.SeedAsync();
    }
}
