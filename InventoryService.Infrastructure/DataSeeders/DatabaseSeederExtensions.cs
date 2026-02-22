using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InventoryService.Infrastructure.DataSeeders;

public static class DatabaseSeederExtensions
{
    /// <summary>
    /// Seeds the database with initial inventory data. Should be called after migrations.
    /// </summary>
    public static async Task SeedInventoryDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<InventoryDataSeeder>>();

        var seeder = new InventoryDataSeeder(context, logger);
        await seeder.SeedAsync();
    }
}
