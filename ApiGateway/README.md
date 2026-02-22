# API Gateway Configuration

## Overview
The API Gateway provides a unified entry point for all microservices in the system using YARP (Yet Another Reverse Proxy).

## Routing Configuration

### User Service
- **Base Path**: `/api/users`
- **Cluster**: `user-cluster`
- **Backend Service**: `user-service`
- **Example Endpoints**:
  - `POST /api/users` - Create user
  - `GET /api/users/{id}` - Get user by ID
  - `GET /api/users` - Get all users
  - `PUT /api/users/{id}` - Update user
  - `DELETE /api/users/{id}` - Delete user

### Order Service
- **Base Path**: `/api/orders`
- **Cluster**: `order-cluster`
- **Backend Service**: `order-service`
- **Example Endpoints**:
  - `POST /api/orders` - Create order
  - `GET /api/orders/{id}` - Get order by ID
  - `GET /api/orders` - Get all orders
  - `POST /api/orders/{id}/items` - Add order item
  - `POST /api/orders/{id}/confirm` - Confirm order
  - `POST /api/orders/{id}/ship` - Ship order
  - `POST /api/orders/{id}/deliver` - Deliver order
  - `POST /api/orders/{id}/cancel` - Cancel order
  - `DELETE /api/orders/{id}` - Delete order

### Inventory Service
- **Base Path**: `/api/inventory`
- **Cluster**: `inventory-cluster`
- **Backend Service**: `inventory-service`
- **Example Endpoints**:
  - `POST /api/inventory` - Create inventory
  - `GET /api/inventory/{id}` - Get inventory by ID
  - `GET /api/inventory/product/{productId}` - Get by product ID
  - `GET /api/inventory/low-stock` - Get low stock items
  - `POST /api/inventory/{id}/adjust` - Adjust quantity
  - `POST /api/inventory/{id}/reserve` - Reserve quantity
  - `POST /api/inventory/{id}/release` - Release reservation
  - `POST /api/inventory/{id}/fulfill` - Fulfill reservation
  - `PUT /api/inventory/{id}/reorder-settings` - Update reorder settings
  - `DELETE /api/inventory/{id}` - Delete inventory

## Running the Application

### Using .NET Aspire AppHost
```bash
# Navigate to the AppHost directory
cd AppHost

# Run the orchestrator
dotnet run
```

This will start:
- **RabbitMQ** (with management UI on port 15672)
- **PostgreSQL** instances for each service (with PgAdmin)
- **Redis** instances for each service (with RedisInsight)
- **UserService** (with its own database and cache)
- **OrderService** (with its own database and cache)
- **InventoryService** (with its own database and cache)
- **API Gateway** (reverse proxy to all services)
- **WebApp** (Blazor frontend)

### Accessing Services

Once the AppHost is running, you can access:
- **API Gateway**: Check the Aspire dashboard for the assigned port (typically https://localhost:7xxx)
- **Aspire Dashboard**: Typically https://localhost:15888 (shows all service endpoints)
- **RabbitMQ Management**: http://localhost:15672 (admin/password from secrets)
- **PgAdmin**: Available through the Aspire dashboard
- **RedisInsight**: Available through the Aspire dashboard

## Testing

Run the PowerShell test script to verify all services:
```powershell
.\test-services.ps1
```

**Note**: Update the `$gatewayUrl` variable in the script with the actual API Gateway port from the Aspire dashboard.

## Architecture

```
┌──────────────┐
│   WebApp     │
│  (Blazor)    │
└──────┬───────┘
       │
       ▼
┌──────────────────────────────────────┐
│         API Gateway (YARP)           │
│  - Route matching                    │
│  - Load balancing                    │
│  - Service discovery via Aspire      │
└──────┬───────────────────────────────┘
       │
       ├─────────┬─────────────┬─────────┐
       ▼         ▼             ▼         ▼
┌─────────┐ ┌─────────┐ ┌──────────┐ ┌─────────┐
│  User   │ │  Order  │ │Inventory │ │RabbitMQ │
│ Service │ │ Service │ │ Service  │ │         │
├─────────┤ ├─────────┤ ├──────────┤ └─────────┘
│   DB    │ │   DB    │ │    DB    │
│  Cache  │ │  Cache  │ │  Cache   │
└─────────┘ └─────────┘ └──────────┘
```

## Configuration

The reverse proxy configuration is defined in `appsettings.json`:
- **Routes**: Define path matching patterns for each service
- **Clusters**: Define backend service addresses (using Aspire service discovery)
- **Transforms**: Optional request/response transformations

## Benefits

1. **Single Entry Point**: Clients only need to know one URL
2. **Service Discovery**: Automatic resolution of backend service addresses via Aspire
3. **Load Balancing**: YARP can distribute traffic across multiple instances
4. **Security**: Centralized authentication/authorization point
5. **Monitoring**: Single point for logging and tracing
6. **Flexibility**: Easy to add new services or change routing without client changes
