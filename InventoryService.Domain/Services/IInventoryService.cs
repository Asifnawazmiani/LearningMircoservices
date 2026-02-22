using InventoryService.Contracts.Requests;
using InventoryService.Contracts.Responses;
using Shared.Common.Results;

namespace InventoryService.Domain.Services;

public interface IInventoryService
{
    Task<Result<ProductInventoryResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProductInventoryResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<ProductInventoryResponse>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductInventoryResponse>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProductInventoryResponse>>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
    Task<Result<ProductInventoryResponse>> CreateInventoryAsync(CreateInventoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductInventoryResponse>> AdjustQuantityAsync(Guid id, AdjustQuantityRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductInventoryResponse>> ReserveQuantityAsync(Guid id, ReserveQuantityRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductInventoryResponse>> ReleaseReservationAsync(Guid id, ReleaseReservationRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductInventoryResponse>> FulfillReservationAsync(Guid id, FulfillReservationRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductInventoryResponse>> UpdateReorderSettingsAsync(Guid id, UpdateReorderSettingsRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteInventoryAsync(Guid id, CancellationToken cancellationToken = default);
}
