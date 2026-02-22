# InventoryService.Domain

This project contains the domain layer for the Inventory Service.

## Structure (to be implemented):
- `/Entities` - Domain entities (e.g., InventoryItemEntity, StockEntity)
- `/ValueObjects` - Value objects (e.g., Quantity, SKU)
- `/Enums` - Domain enums (e.g., StockStatus)
- `/Events` - Domain events (e.g., StockLevelChangedEvent)
- `/Repositories` - Repository interfaces
- `/Services` - Domain service interfaces
- `/Exceptions` - Domain-specific exceptions

## Dependencies:
- Should only reference: Infrastructure.Persistence (for BaseEntity)
- Should NOT reference: EF Core, or any infrastructure concerns
