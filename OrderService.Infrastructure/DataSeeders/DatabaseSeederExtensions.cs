using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OrderService.Infrastructure.DataSeeders;

public static class DatabaseSeederExtensions
{
    /// <summary>
    /// Seeds the database with initial order data. Should be called after migrations.
    /// Optionally accepts customer IDs and product catalog for realistic data.
    /// </summary>
    public static async Task SeedOrderDatabaseAsync(
        this IServiceProvider serviceProvider,
        List<Guid>? customerIds = null,
        Dictionary<string, Guid>? productCatalog = null)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<OrderDataSeeder>>();

        var seeder = new OrderDataSeeder(context, logger);
        await seeder.SeedAsync(customerIds, productCatalog);
    }
}
