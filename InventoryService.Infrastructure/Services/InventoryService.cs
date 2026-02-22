using InventoryService.Contracts.Requests;
using InventoryService.Contracts.Responses;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;
using Shared.Common.Results;
using Shared.Domain.UnitOfWork;

namespace InventoryService.Infrastructure.Services;

public class InventoryService : Domain.Services.IInventoryService
{
    private readonly IProductInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(IProductInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductInventoryResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id, cancellationToken);
        if (inventory is null)
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.NotFound", $"Inventory with ID {id} not found"));

        return Result<ProductInventoryResponse>.Success(MapToResponse(inventory));
    }

    public async Task<Result<IEnumerable<ProductInventoryResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var inventories = await _inventoryRepository.GetAllAsync(cancellationToken);
        var responses = inventories.Select(MapToResponse);
        return Result<IEnumerable<ProductInventoryResponse>>.Success(responses);
    }

    public async Task<Result<ProductInventoryResponse>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(productId, cancellationToken);
        if (inventory is null)
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.NotFound", $"Inventory for product {productId} not found"));

        return Result<ProductInventoryResponse>.Success(MapToResponse(inventory));
    }

    public async Task<Result<ProductInventoryResponse>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.GetBySkuAsync(sku, cancellationToken);
        if (inventory is null)
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.NotFound", $"Inventory with SKU {sku} not found"));

        return Result<ProductInventoryResponse>.Success(MapToResponse(inventory));
    }

    public async Task<Result<IEnumerable<ProductInventoryResponse>>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        var inventories = await _inventoryRepository.GetLowStockItemsAsync(cancellationToken);
        var responses = inventories.Select(MapToResponse);
        return Result<IEnumerable<ProductInventoryResponse>>.Success(responses);
    }

    public async Task<Result<ProductInventoryResponse>> CreateInventoryAsync(CreateInventoryRequest request, CancellationToken cancellationToken = default)
    {
        var existingByProductId = await _inventoryRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (existingByProductId is not null)
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.DuplicateProduct", $"Inventory for product {request.ProductId} already exists"));

        var existingBySku = await _inventoryRepository.GetBySkuAsync(request.Sku, cancellationToken);
        if (existingBySku is not null)
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.DuplicateSku", $"Inventory with SKU {request.Sku} already exists"));

        try
        {
            var inventory = ProductInventoryEntity.Create(
                request.ProductId,
                request.ProductName,
                request.Sku,
                request.InitialQuantity,
                request.ReorderLevel,
                request.ReorderQuantity,
                request.Location);

            await _inventoryRepository.AddAsync(inventory, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ProductInventoryResponse>.Success(MapToResponse(inventory));
        }
        catch (ArgumentException ex)
        {
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.ValidationError", ex.Message));
        }
    }

    public async Task<Result<ProductInventoryResponse>> AdjustQuantityAsync(Guid id, AdjustQuantityRequest request, CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id, cancellationToken);
        if (inventory is null)
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.NotFound", $"Inventory with ID {id} not found"));

        try
        {
            inventory.AdjustQuantity(request.Adjustment, request.Reason);
            _inventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ProductInventoryResponse>.Success(MapToResponse(inventory));
        }
        catch (InvalidOperationException ex)
        {
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.InvalidOperation", ex.Message));
        }
    }

    public async Task<Result<ProductInventoryResponse>> ReserveQuantityAsync(Guid id, ReserveQuantityRequest request, CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id, cancellationToken);
        if (inventory is null)
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.NotFound", $"Inventory with ID {id} not found"));

        try
        {
            inventory.ReserveQuantity(request.Quantity);
            _inventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ProductInventoryResponse>.Success(MapToResponse(inventory));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.InvalidOperation", ex.Message));
        }
    }

    public async Task<Result<ProductInventoryResponse>> ReleaseReservationAsync(Guid id, ReleaseReservationRequest request, CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id, cancellationToken);
        if (inventory is null)
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.NotFound", $"Inventory with ID {id} not found"));

        try
        {
            inventory.ReleaseReservation(request.Quantity);
            _inventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ProductInventoryResponse>.Success(MapToResponse(inventory));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.InvalidOperation", ex.Message));
        }
    }

    public async Task<Result<ProductInventoryResponse>> FulfillReservationAsync(Guid id, FulfillReservationRequest request, CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id, cancellationToken);
        if (inventory is null)
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.NotFound", $"Inventory with ID {id} not found"));

        try
        {
            inventory.FulfillReservation(request.Quantity);
            _inventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ProductInventoryResponse>.Success(MapToResponse(inventory));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.InvalidOperation", ex.Message));
        }
    }

    public async Task<Result<ProductInventoryResponse>> UpdateReorderSettingsAsync(Guid id, UpdateReorderSettingsRequest request, CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id, cancellationToken);
        if (inventory is null)
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.NotFound", $"Inventory with ID {id} not found"));

        try
        {
            inventory.UpdateReorderSettings(request.ReorderLevel, request.ReorderQuantity);
            _inventoryRepository.Update(inventory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ProductInventoryResponse>.Success(MapToResponse(inventory));
        }
        catch (ArgumentException ex)
        {
            return (Result<ProductInventoryResponse>)Result.Failure<ProductInventoryResponse>(new Error("Inventory.ValidationError", ex.Message));
        }
    }

    public async Task<Result> DeleteInventoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id, cancellationToken);
        if (inventory is null)
            return Result.Failure(new Error("Inventory.NotFound", $"Inventory with ID {id} not found"));

        _inventoryRepository.Delete(inventory);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static ProductInventoryResponse MapToResponse(ProductInventoryEntity inventory)
    {
        return new ProductInventoryResponse
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            ProductName = inventory.ProductName,
            Sku = inventory.Sku,
            QuantityOnHand = inventory.QuantityOnHand,
            ReservedQuantity = inventory.ReservedQuantity,
            AvailableQuantity = inventory.AvailableQuantity,
            ReorderLevel = inventory.ReorderLevel,
            ReorderQuantity = inventory.ReorderQuantity,
            Location = inventory.Location,
            NeedsReorder = inventory.NeedsReorder(),
            CreatedAt = inventory.CreatedAt,
            UpdatedAt = inventory.UpdatedAt
        };
    }
}



