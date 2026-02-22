using OrderService.Contracts.Requests;
using OrderService.Contracts.Responses;
using Shared.Common.Results;

namespace OrderService.Domain.Services;

public interface IOrderService
{
    Task<Result<OrderResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<OrderResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<OrderResponse>>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> AddOrderItemAsync(Guid orderId, AddOrderItemRequest request, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> RemoveOrderItemAsync(Guid orderId, Guid itemId, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> ConfirmOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> ShipOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> DeliverOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result> DeleteOrderAsync(Guid id, CancellationToken cancellationToken = default);
}
