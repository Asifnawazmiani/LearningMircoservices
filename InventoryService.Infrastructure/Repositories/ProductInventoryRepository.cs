using Infrastructure.Persistence.Repositories;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

public class ProductInventoryRepository : BaseRepository<ProductInventoryEntity, Guid>, IProductInventoryRepository
{
    private readonly InventoryDbContext _context;

    public ProductInventoryRepository(InventoryDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ProductInventoryEntity?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductInventories
            .FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken);
    }

    public async Task<ProductInventoryEntity?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _context.ProductInventories
            .FirstOrDefaultAsync(p => p.Sku == sku, cancellationToken);
    }

    public async Task<IEnumerable<ProductInventoryEntity>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductInventories
            .Where(p => (p.QuantityOnHand - p.ReservedQuantity) <= p.ReorderLevel)
            .OrderBy(p => p.QuantityOnHand - p.ReservedQuantity)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductInventoryEntity>> GetByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        return await _context.ProductInventories
            .Where(p => p.Location == location)
            .OrderBy(p => p.ProductName)
            .ToListAsync(cancellationToken);
    }
}
