using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.DataSeeders;

public class OrderDataSeeder
{
    private readonly OrderDbContext _context;
    private readonly ILogger<OrderDataSeeder> _logger;

    public OrderDataSeeder(OrderDbContext context, ILogger<OrderDataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seed sample orders. In a real scenario, you would pass actual customer IDs and product IDs
    /// from UserService and InventoryService.
    /// </summary>
    public async Task SeedAsync(
        List<Guid>? customerIds = null, 
        Dictionary<string, Guid>? productCatalog = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if data already exists
            if (await _context.Orders.AnyAsync(cancellationToken))
            {
                _logger.LogInformation("Order data already exists. Skipping seed.");
                return;
            }

            _logger.LogInformation("Starting order data seeding...");

            // Use provided customer IDs or generate sample ones
            var customers = customerIds ?? GenerateSampleCustomerIds(5);
            
            // Use provided product catalog or generate sample ones
            var products = productCatalog ?? GenerateSampleProductCatalog();

            var orders = new List<OrderEntity>();
            var orderNumber = 1000;

            // Order 1 - Completed order
            var order1 = OrderEntity.Create(customers[0], $"ORD-{orderNumber++}");
            order1.AddItem(products["Wireless Headphones"], "Wireless Headphones", 2, 299.99m);
            order1.AddItem(products["Laptop Stand"], "Laptop Stand", 1, 49.99m);
            order1.ConfirmOrder();
            order1.Ship();
            order1.Deliver();
            orders.Add(order1);

            // Order 2 - Shipped order
            var order2 = OrderEntity.Create(customers[1], $"ORD-{orderNumber++}");
            order2.AddItem(products["Smart Watch"], "Smart Watch", 1, 399.99m);
            order2.AddItem(products["Running Shoes"], "Running Shoes", 1, 129.99m);
            order2.ConfirmOrder();
            order2.Ship();
            orders.Add(order2);

            // Order 3 - Confirmed order
            var order3 = OrderEntity.Create(customers[2], $"ORD-{orderNumber++}");
            order3.AddItem(products["Coffee Maker"], "Coffee Maker", 1, 89.99m);
            order3.AddItem(products["Blender Pro"], "Blender Pro", 1, 149.99m);
            order3.ConfirmOrder();
            orders.Add(order3);

            // Order 4 - Pending order
            var order4 = OrderEntity.Create(customers[3], $"ORD-{orderNumber++}");
            order4.AddItem(products["Yoga Mat Premium"], "Yoga Mat Premium", 2, 39.99m);
            order4.AddItem(products["Air Fryer"], "Air Fryer", 1, 119.99m);
            orders.Add(order4);

            // Order 5 - Another pending order
            var order5 = OrderEntity.Create(customers[4], $"ORD-{orderNumber++}");
            order5.AddItem(products["Clean Code Book"], "Clean Code Book", 3, 49.99m);
            order5.AddItem(products["Design Patterns Book"], "Design Patterns Book", 2, 54.99m);
            orders.Add(order5);

            // Order 6 - Cancelled order
            var order6 = OrderEntity.Create(customers[0], $"ORD-{orderNumber++}");
            order6.AddItem(products["Smart Watch"], "Smart Watch", 1, 399.99m);
            order6.Cancel();
            orders.Add(order6);

            // Order 7 - Large order
            var order7 = OrderEntity.Create(customers[1], $"ORD-{orderNumber++}");
            order7.AddItem(products["Wireless Headphones"], "Wireless Headphones", 5, 299.99m);
            order7.AddItem(products["Laptop Stand"], "Laptop Stand", 5, 49.99m);
            order7.AddItem(products["Coffee Maker"], "Coffee Maker", 3, 89.99m);
            order7.ConfirmOrder();
            orders.Add(order7);

            // Order 8 - Single item order
            var order8 = OrderEntity.Create(customers[2], $"ORD-{orderNumber++}");
            order8.AddItem(products["Running Shoes"], "Running Shoes", 1, 129.99m);
            order8.ConfirmOrder();
            order8.Ship();
            order8.Deliver();
            orders.Add(order8);

            await _context.Orders.AddRangeAsync(orders, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully seeded {Count} orders with various statuses", orders.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding order data");
            throw;
        }
    }

    private List<Guid> GenerateSampleCustomerIds(int count)
    {
        var ids = new List<Guid>();
        for (int i = 0; i < count; i++)
        {
            ids.Add(Guid.NewGuid());
        }
        return ids;
    }

    private Dictionary<string, Guid> GenerateSampleProductCatalog()
    {
        return new Dictionary<string, Guid>
        {
            { "Wireless Headphones", Guid.NewGuid() },
            { "Smart Watch", Guid.NewGuid() },
            { "Laptop Stand", Guid.NewGuid() },
            { "Coffee Maker", Guid.NewGuid() },
            { "Blender Pro", Guid.NewGuid() },
            { "Air Fryer", Guid.NewGuid() },
            { "Yoga Mat Premium", Guid.NewGuid() },
            { "Running Shoes", Guid.NewGuid() },
            { "Clean Code Book", Guid.NewGuid() },
            { "Design Patterns Book", Guid.NewGuid() }
        };
    }
}
