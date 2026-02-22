# OrderService.Domain

This project contains the domain layer for the Order Service.

## Structure (to be implemented):
- `/Entities` - Domain entities (e.g., OrderEntity, OrderItemEntity)
- `/ValueObjects` - Value objects (e.g., Money, OrderNumber)
- `/Enums` - Domain enums (e.g., OrderStatus)
- `/Events` - Domain events (e.g., OrderCreatedEvent)
- `/Repositories` - Repository interfaces
- `/Services` - Domain service interfaces
- `/Exceptions` - Domain-specific exceptions

## Dependencies:
- Should only reference: Infrastructure.Persistence (for BaseEntity)
- Should NOT reference: EF Core, or any infrastructure concerns
