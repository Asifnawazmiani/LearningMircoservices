using InventoryService.Domain.Entities;
using Shared.Domain.Repositories;

namespace InventoryService.Domain.Repositories;

public interface IProductInventoryRepository : IBaseRepository<ProductInventoryEntity, Guid>
{
    Task<ProductInventoryEntity?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<ProductInventoryEntity?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductInventoryEntity>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductInventoryEntity>> GetByLocationAsync(string location, CancellationToken cancellationToken = default);
}
