using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryService.Infrastructure.DataSeeders;

public class InventoryDataSeeder
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<InventoryDataSeeder> _logger;

    public InventoryDataSeeder(InventoryDbContext context, ILogger<InventoryDataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if data already exists
            if (await _context.ProductInventories.AnyAsync(cancellationToken))
            {
                _logger.LogInformation("Inventory data already exists. Skipping seed.");
                return;
            }

            _logger.LogInformation("Starting inventory data seeding...");

            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();
            var productId3 = Guid.NewGuid();
            var productId4 = Guid.NewGuid();
            var productId5 = Guid.NewGuid();
            var productId6 = Guid.NewGuid();
            var productId7 = Guid.NewGuid();
            var productId8 = Guid.NewGuid();
            var productId9 = Guid.NewGuid();
            var productId10 = Guid.NewGuid();

            var inventories = new List<ProductInventoryEntity>
            {
                // Electronics
                ProductInventoryEntity.Create(
                    productId1,
                    "Wireless Headphones",
                    "WH-1000XM4",
                    150,
                    30,
                    50,
                    "Warehouse A - Section 1"
                ),
                ProductInventoryEntity.Create(
                    productId2,
                    "Smart Watch",
                    "SW-SERIES-7",
                    200,
                    40,
                    75,
                    "Warehouse A - Section 2"
                ),
                ProductInventoryEntity.Create(
                    productId3,
                    "Laptop Stand",
                    "LS-ERGO-01",
                    300,
                    50,
                    100,
                    "Warehouse A - Section 3"
                ),

                // Home & Kitchen
                ProductInventoryEntity.Create(
                    productId4,
                    "Coffee Maker",
                    "CM-BREW-2023",
                    80,
                    15,
                    30,
                    "Warehouse B - Section 1"
                ),
                ProductInventoryEntity.Create(
                    productId5,
                    "Blender Pro",
                    "BL-PRO-500",
                    120,
                    25,
                    40,
                    "Warehouse B - Section 2"
                ),
                ProductInventoryEntity.Create(
                    productId6,
                    "Air Fryer",
                    "AF-HEAT-2024",
                    90,
                    20,
                    35,
                    "Warehouse B - Section 3"
                ),

                // Sports & Outdoors
                ProductInventoryEntity.Create(
                    productId7,
                    "Yoga Mat Premium",
                    "YM-PREMIUM",
                    250,
                    50,
                    80,
                    "Warehouse C - Section 1"
                ),
                ProductInventoryEntity.Create(
                    productId8,
                    "Running Shoes",
                    "RS-ULTRA-42",
                    100,
                    20,
                    40,
                    "Warehouse C - Section 2"
                ),

                // Books & Media
                ProductInventoryEntity.Create(
                    productId9,
                    "Clean Code Book",
                    "BOOK-CC-2023",
                    500,
                    100,
                    150,
                    "Warehouse D - Section 1"
                ),
                ProductInventoryEntity.Create(
                    productId10,
                    "Design Patterns Book",
                    "BOOK-DP-2023",
                    450,
                    90,
                    140,
                    "Warehouse D - Section 2"
                )
            };

            // Reserve some quantities to simulate real scenarios
            inventories[0].ReserveQuantity(10); // Wireless Headphones
            inventories[1].ReserveQuantity(15); // Smart Watch
            inventories[3].ReserveQuantity(5);  // Coffee Maker
            inventories[7].ReserveQuantity(8);  // Running Shoes

            await _context.ProductInventories.AddRangeAsync(inventories, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully seeded {Count} inventory items", inventories.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding inventory data");
            throw;
        }
    }

    public async Task<Dictionary<string, Guid>> GetSeededProductIdsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _context.ProductInventories
            .OrderBy(p => p.ProductName)
            .Select(p => new { p.ProductName, p.ProductId })
            .ToListAsync(cancellationToken);

        return products.ToDictionary(p => p.ProductName, p => p.ProductId);
    }
}
