using Shared.Domain.Entities;

namespace InventoryService.Domain.Entities;

public class ProductInventoryEntity : GuidEntity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string Sku { get; private set; } = string.Empty;
    public int QuantityOnHand { get; private set; }
    public int ReservedQuantity { get; private set; }
    public int AvailableQuantity => QuantityOnHand - ReservedQuantity;
    public int ReorderLevel { get; private set; }
    public int ReorderQuantity { get; private set; }
    public string Location { get; private set; } = string.Empty;

    private ProductInventoryEntity() { }

    public static ProductInventoryEntity Create(
        Guid productId,
        string productName,
        string sku,
        int initialQuantity,
        int reorderLevel,
        int reorderQuantity,
        string location)
    {
        if (initialQuantity < 0)
            throw new ArgumentException("Initial quantity cannot be negative", nameof(initialQuantity));

        if (reorderLevel < 0)
            throw new ArgumentException("Reorder level cannot be negative", nameof(reorderLevel));

        if (reorderQuantity <= 0)
            throw new ArgumentException("Reorder quantity must be greater than zero", nameof(reorderQuantity));

        return new ProductInventoryEntity
        {
            ProductId = productId,
            ProductName = productName,
            Sku = sku,
            QuantityOnHand = initialQuantity,
            ReservedQuantity = 0,
            ReorderLevel = reorderLevel,
            ReorderQuantity = reorderQuantity,
            Location = location
        };
    }

    public void AdjustQuantity(int adjustment, string reason)
    {
        var newQuantity = QuantityOnHand + adjustment;
        if (newQuantity < 0)
            throw new InvalidOperationException($"Cannot adjust quantity by {adjustment}. Would result in negative inventory.");

        QuantityOnHand = newQuantity;
    }

    public void ReserveQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity to reserve must be greater than zero", nameof(quantity));

        if (AvailableQuantity < quantity)
            throw new InvalidOperationException($"Insufficient inventory. Available: {AvailableQuantity}, Requested: {quantity}");

        ReservedQuantity += quantity;
    }

    public void ReleaseReservation(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity to release must be greater than zero", nameof(quantity));

        if (ReservedQuantity < quantity)
            throw new InvalidOperationException($"Cannot release {quantity} units. Only {ReservedQuantity} units are reserved.");

        ReservedQuantity -= quantity;
    }

    public void FulfillReservation(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity to fulfill must be greater than zero", nameof(quantity));

        if (ReservedQuantity < quantity)
            throw new InvalidOperationException($"Cannot fulfill {quantity} units. Only {ReservedQuantity} units are reserved.");

        ReservedQuantity -= quantity;
        QuantityOnHand -= quantity;
    }

    public bool NeedsReorder() => AvailableQuantity <= ReorderLevel;

    public void UpdateReorderSettings(int reorderLevel, int reorderQuantity)
    {
        if (reorderLevel < 0)
            throw new ArgumentException("Reorder level cannot be negative", nameof(reorderLevel));

        if (reorderQuantity <= 0)
            throw new ArgumentException("Reorder quantity must be greater than zero", nameof(reorderQuantity));

        ReorderLevel = reorderLevel;
        ReorderQuantity = reorderQuantity;
    }

    public void UpdateLocation(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty", nameof(location));

        Location = location;
    }
}
