using OrderService.Contracts.Requests;
using OrderService.Contracts.Responses;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;
using Shared.Common.Results;
using Shared.Domain.UnitOfWork;

namespace OrderService.Infrastructure.Services;

public class OrderService : Domain.Services.IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null)
            return Result<OrderResponse>.Failure(new Error("Order.NotFound", $"Order with ID {id} not found"));

        return Result<OrderResponse>.Success(MapToResponse(order));
    }

    public async Task<Result<IEnumerable<OrderResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        var responses = orders.Select(MapToResponse);
        return Result<IEnumerable<OrderResponse>>.Success(responses);
    }

    public async Task<Result<OrderResponse>> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByOrderNumberAsync(orderNumber, cancellationToken);
        if (order is null)
            return Result<OrderResponse>.Failure(new Error("Order.NotFound", $"Order with number {orderNumber} not found"));

        return Result<OrderResponse>.Success(MapToResponse(order));
    }

    public async Task<Result<IEnumerable<OrderResponse>>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        var responses = orders.Select(MapToResponse);
        return Result<IEnumerable<OrderResponse>>.Success(responses);
    }

    public async Task<Result<OrderResponse>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var existingOrder = await _orderRepository.GetByOrderNumberAsync(request.OrderNumber, cancellationToken);
        if (existingOrder is not null)
            return Result<OrderResponse>.Failure(new Error("Order.DuplicateOrderNumber", $"Order with number {request.OrderNumber} already exists"));

        var order = OrderEntity.Create(request.CustomerId, request.OrderNumber);
        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<OrderResponse>.Success(MapToResponse(order));
    }

    public async Task<Result<OrderResponse>> AddOrderItemAsync(Guid orderId, AddOrderItemRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
            return Result<OrderResponse>.Failure(new Error("Order.NotFound", $"Order with ID {orderId} not found"));

        try
        {
            order.AddItem(request.ProductId, request.ProductName, request.Quantity, request.UnitPrice);
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<OrderResponse>.Success(MapToResponse(order));
        }
        catch (InvalidOperationException ex)
        {
            return Result<OrderResponse>.Failure(new Error("Order.InvalidOperation", ex.Message));
        }
    }

    public async Task<Result<OrderResponse>> RemoveOrderItemAsync(Guid orderId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
            return Result<OrderResponse>.Failure(new Error("Order.NotFound", $"Order with ID {orderId} not found"));

        order.RemoveItem(itemId);
        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<OrderResponse>.Success(MapToResponse(order));
    }

    public async Task<Result<OrderResponse>> ConfirmOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
            return Result<OrderResponse>.Failure(new Error("Order.NotFound", $"Order with ID {orderId} not found"));

        try
        {
            order.ConfirmOrder();
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<OrderResponse>.Success(MapToResponse(order));
        }
        catch (InvalidOperationException ex)
        {
            return Result<OrderResponse>.Failure(new Error("Order.InvalidOperation", ex.Message));
        }
    }

    public async Task<Result<OrderResponse>> ShipOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
            return Result<OrderResponse>.Failure(new Error("Order.NotFound", $"Order with ID {orderId} not found"));

        try
        {
            order.Ship();
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<OrderResponse>.Success(MapToResponse(order));
        }
        catch (InvalidOperationException ex)
        {
            return Result<OrderResponse>.Failure(new Error("Order.InvalidOperation", ex.Message));
        }
    }

    public async Task<Result<OrderResponse>> DeliverOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
            return Result<OrderResponse>.Failure(new Error("Order.NotFound", $"Order with ID {orderId} not found"));

        try
        {
            order.Deliver();
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<OrderResponse>.Success(MapToResponse(order));
        }
        catch (InvalidOperationException ex)
        {
            return Result<OrderResponse>.Failure(new Error("Order.InvalidOperation", ex.Message));
        }
    }

    public async Task<Result<OrderResponse>> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
            return Result<OrderResponse>.Failure(new Error("Order.NotFound", $"Order with ID {orderId} not found"));

        try
        {
            order.Cancel();
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<OrderResponse>.Success(MapToResponse(order));
        }
        catch (InvalidOperationException ex)
        {
            return Result<OrderResponse>.Failure(new Error("Order.InvalidOperation", ex.Message));
        }
    }

    public async Task<Result> DeleteOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null)
            return Result.Failure(new Error("Order.NotFound", $"Order with ID {id} not found"));

        _orderRepository.Delete(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static OrderResponse MapToResponse(OrderEntity order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            ShippedDate = order.ShippedDate,
            DeliveredDate = order.DeliveredDate,
            Items = order.Items.Select(i => new OrderItemResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }
}


