using Shared.Domain.Entities;

namespace OrderService.Domain.Entities;

public class OrderEntity : GuidEntity
{
    public Guid CustomerId { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ShippedDate { get; private set; }
    public DateTime? DeliveredDate { get; private set; }

    private readonly List<OrderItemEntity> _items = new();
    public IReadOnlyCollection<OrderItemEntity> Items => _items.AsReadOnly();

    private OrderEntity() { }

    public static OrderEntity Create(Guid customerId, string orderNumber)
    {
        return new OrderEntity
        {
            CustomerId = customerId,
            OrderNumber = orderNumber,
            Status = OrderStatus.Pending,
            OrderDate = DateTime.UtcNow,
            TotalAmount = 0
        };
    }

    public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        var item = OrderItemEntity.Create(Id, productId, productName, quantity, unitPrice);
        _items.Add(item);
        RecalculateTotal();
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotal();
        }
    }

    public void ConfirmOrder()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");
        
        Status = OrderStatus.Confirmed;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be shipped");
        
        Status = OrderStatus.Shipped;
        ShippedDate = DateTime.UtcNow;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be delivered");
        
        Status = OrderStatus.Delivered;
        DeliveredDate = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Delivered orders cannot be cancelled");
        
        Status = OrderStatus.Cancelled;
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.TotalPrice);
    }
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}
