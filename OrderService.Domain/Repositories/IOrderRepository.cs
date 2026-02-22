using OrderService.Domain.Entities;
using Shared.Domain.Repositories;

namespace OrderService.Domain.Repositories;

public interface IOrderRepository : IBaseRepository<OrderEntity, Guid>
{
    Task<OrderEntity?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderEntity>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderEntity>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
}
