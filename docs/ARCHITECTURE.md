# Architecture Documentation

## Overview

This document provides detailed architectural information about the Learning Microservices project. The solution implements a modern microservices architecture using .NET 10, Clean Architecture principles, and Domain-Driven Design patterns.

---

## Table of Contents

1. [Architectural Principles](#architectural-principles)
2. [System Architecture](#system-architecture)
3. [Service Architecture](#service-architecture)
4. [Data Architecture](#data-architecture)
5. [Messaging Architecture](#messaging-architecture)
6. [Infrastructure Architecture](#infrastructure-architecture)
7. [Deployment Architecture](#deployment-architecture)

---

## Architectural Principles

### Core Principles

1. **Separation of Concerns**: Each layer has a specific responsibility
2. **Dependency Inversion**: High-level modules don't depend on low-level modules
3. **Single Responsibility**: Each service has one reason to change
4. **Database per Service**: Each microservice owns its data
5. **Event-Driven Communication**: Asynchronous messaging for inter-service communication
6. **API Gateway Pattern**: Single entry point for clients
7. **Service Discovery**: Dynamic service location via Aspire

### Architectural Constraints

- **No Direct Database Access**: Services can only access their own database
- **No Direct Service-to-Service Calls** (except via HTTP Client SDK): Use events for async communication
- **No Shared Database**: Each service has complete control over its schema
- **Backward Compatibility**: API changes must be backward compatible
- **Idempotency**: All event handlers must be idempotent

---

## System Architecture

### C4 Model - System Context

```mermaid
graph TB
    subgraph External["External Actors"]
        USER[👤 End User]
        ADMIN[👨‍💼 Administrator]
    end

    subgraph System["Learning Microservices System"]
        WEBAPP[WebApp<br/>Blazor SPA]
        GATEWAY[API Gateway<br/>YARP]
        
        subgraph Services["Microservices"]
            US[User Service<br/>Authentication & User Management]
            OS[Order Service<br/>Order Processing]
            IS[Inventory Service<br/>Stock Management]
        end
    end

    subgraph External_Systems["External Systems"]
        EMAIL[📧 Email Service<br/>SMTP]
        PAYMENT[💳 Payment Gateway<br/>Stripe/PayPal]
    end

    USER -->|Uses| WEBAPP
    ADMIN -->|Manages| WEBAPP
    WEBAPP -->|API Calls| GATEWAY
    GATEWAY --> US
    GATEWAY --> OS
    GATEWAY --> IS
    
    US -.->|Sends emails| EMAIL
    OS -.->|Process payments| PAYMENT

    style USER fill:#e3f2fd
    style ADMIN fill:#f3e5f5
    style System fill:#e8f5e9
    style External_Systems fill:#fff3e0
```

### C4 Model - Container Diagram

```mermaid
graph TB
    subgraph Browser["User's Browser"]
        SPA[Single Page Application<br/>Blazor WebAssembly<br/>C#, HTML, CSS]
    end

    subgraph AspireHost["Aspire Orchestration"]
        ASPIRE[AppHost<br/>.NET Aspire<br/>Service Orchestration & Discovery]
    end

    subgraph APIGateway["API Gateway Container"]
        YARP[YARP Reverse Proxy<br/>.NET 10<br/>Routing, Caching, Auth]
    end

    subgraph UserService["User Service Container"]
        US_API[ASP.NET Core API<br/>.NET 10<br/>Controllers, Middleware]
        US_DOMAIN[Domain Layer<br/>Entities, VOs, Events]
        US_INFRA[Infrastructure<br/>EF Core, Repositories]
    end

    subgraph OrderService["Order Service Container"]
        OS_API[ASP.NET Core API<br/>.NET 10<br/>Controllers, Middleware]
        OS_DOMAIN[Domain Layer<br/>Entities, VOs, Events]
        OS_INFRA[Infrastructure<br/>EF Core, Repositories]
    end

    subgraph InventoryService["Inventory Service Container"]
        IS_API[ASP.NET Core API<br/>.NET 10<br/>Controllers, Middleware]
        IS_DOMAIN[Domain Layer<br/>Entities, VOs, Events]
        IS_INFRA[Infrastructure<br/>EF Core, Repositories]
    end

    subgraph Databases["Databases"]
        USER_DB[(User DB<br/>PostgreSQL)]
        ORDER_DB[(Order DB<br/>PostgreSQL)]
        INV_DB[(Inventory DB<br/>PostgreSQL)]
    end

    subgraph Infrastructure["Infrastructure Services"]
        RABBIT[Message Broker<br/>RabbitMQ<br/>AMQP 3.12]
        REDIS[Cache<br/>Redis<br/>7.2]
    end

    SPA -->|HTTPS/JSON| YARP
    YARP -->|Routes /users/*| US_API
    YARP -->|Routes /orders/*| OS_API
    YARP -->|Routes /inventory/*| IS_API
    YARP <-->|Cache| REDIS

    US_API --> US_DOMAIN
    US_DOMAIN --> US_INFRA
    US_INFRA -->|Npgsql| USER_DB

    OS_API --> OS_DOMAIN
    OS_DOMAIN --> OS_INFRA
    OS_INFRA -->|Npgsql| ORDER_DB

    IS_API --> IS_DOMAIN
    IS_DOMAIN --> IS_INFRA
    IS_INFRA -->|Npgsql| INV_DB

    US_API & OS_API & IS_API -->|MassTransit| RABBIT

    ASPIRE -.->|Orchestrates| SPA & YARP & US_API & OS_API & IS_API
    ASPIRE -.->|Provisions| USER_DB & ORDER_DB & INV_DB & RABBIT & REDIS
```

---

## Service Architecture

### Clean Architecture Layers

Each microservice follows the Clean Architecture pattern with the following layers:

```
Service/
├── API Layer (Presentation)
│   ├── Controllers
│   ├── Middlewares
│   └── Program.cs (DI, Configuration)
│
├── Contracts Layer
│   ├── DTOs (Data Transfer Objects)
│   ├── Requests
│   └── Responses
│
├── Domain Layer (Core Business Logic)
│   ├── Entities (Aggregates)
│   ├── Value Objects
│   ├── Domain Events
│   ├── Domain Exceptions
│   └── Repository Interfaces
│
└── Infrastructure Layer
    ├── DbContext
    ├── Entity Configurations (Fluent API)
    ├── Repository Implementations
    └── Migrations
```

### Dependency Flow

```mermaid
graph TB
    API[API Layer<br/>Controllers, Middlewares]
    CON[Contracts Layer<br/>DTOs, Requests]
    DOM[Domain Layer<br/>Entities, VOs, Events]
    INF[Infrastructure Layer<br/>DbContext, Repos]
    EXT[External Dependencies<br/>EF Core, Npgsql]

    API -->|depends on| DOM
    API -->|depends on| CON
    API -->|depends on| INF
    INF -->|depends on| DOM
    INF -->|depends on| EXT
    
    DOM -.->|independent| EXT

    style DOM fill:#c8e6c9
    style API fill:#e3f2fd
    style INF fill:#fff3e0
```

**Key Points:**
- Domain layer has **zero external dependencies**
- Infrastructure depends on Domain (not vice versa)
- API layer orchestrates between layers
- All dependencies point **inward** toward the domain

---

## Data Architecture

### Database Schema Strategy

Each microservice has its own PostgreSQL database following these conventions:

#### User Service Schema

```mermaid
erDiagram
    Users ||--o{ UserRoles : has
    Users {
        uuid Id PK
        string Email
        string FirstName
        string LastName
        string MobileNo
        bool IsEmailVerified
        bool IsMobileVerified
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
        datetime DeletedAt
    }
    UserRoles {
        uuid Id PK
        uuid UserId FK
        string Role
        datetime GrantedAt
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
        datetime DeletedAt
    }
    OutboxMessages {
        uuid Id PK
        string EventType
        jsonb Payload
        datetime CreatedAt
        bool IsPublished
        datetime PublishedAt
        int Retries
    }
```

#### Shared Base Entity Pattern

All entities inherit from `BaseEntity<TId>` (defined in `Shared.Domain`):

```csharp
public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
}
```

**Benefits:**
- Consistent ID generation (GUID v7)
- Automatic audit fields
- Built-in soft delete support
- Timestamp tracking

### Data Consistency Patterns

#### Transactional Consistency (Within Service)

```mermaid
sequenceDiagram
    participant API as API Controller
    participant DOM as Domain Entity
    participant UOW as Unit of Work
    participant DB as PostgreSQL

    API->>DOM: Perform business operation
    DOM->>DOM: Validate business rules
    DOM->>DOM: Raise domain event
    API->>UOW: SaveChangesAsync()
    UOW->>DB: BEGIN TRANSACTION
    UOW->>DB: INSERT/UPDATE entity
    UOW->>DB: INSERT outbox message
    UOW->>DB: COMMIT TRANSACTION
    DB-->>API: Success
```

#### Eventual Consistency (Between Services)

```mermaid
sequenceDiagram
    participant OS as Order Service
    participant ODB as Order DB
    participant W as Outbox Worker
    participant MQ as RabbitMQ
    participant IS as Inventory Service
    participant IDB as Inventory DB

    OS->>ODB: Save order + event to outbox (Transaction)
    Note over OS,ODB: Transactional consistency

    W->>ODB: Poll outbox messages
    W->>MQ: Publish OrderCreatedEvent
    W->>ODB: Mark as published

    MQ->>IS: OrderCreatedEvent
    IS->>IDB: Reserve inventory
    Note over IS,IDB: Eventual consistency
    IDB-->>IS: Success
    IS->>MQ: InventoryReservedEvent
```

---

## Messaging Architecture

### Event Types

#### 1. Domain Events (Intra-Service)

**Purpose**: Communication within a single service
**Scope**: Internal to service
**Storage**: In-memory (cleared after processing)
**Example**: `UserEmailVerifiedEvent`

```csharp
// Raised within domain entity
public class UserEntity : GuidEntity
{
    public void VerifyEmail()
    {
        IsEmailVerified = true;
        Raise(new UserEmailVerifiedEvent(Id, Email));
    }
}

// Processed before SaveChanges
// Converted to OutboxMessage
```

#### 2. Integration Events (Inter-Service)

**Purpose**: Communication between services
**Scope**: Cross-service
**Storage**: RabbitMQ via MassTransit
**Example**: `OrderCreatedEvent`

```csharp
// Published to RabbitMQ
public record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public decimal TotalAmount { get; init; }
    public List<OrderItem> Items { get; init; }
}

// Consumed by InventoryService
public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        // Reserve inventory
    }
}
```

### Message Flow Architecture

```mermaid
graph TB
    subgraph ServiceA["Order Service"]
        A_ENT[Order Entity]
        A_DOM[Domain Event]
        A_CTX[DbContext]
        A_OUT[Outbox Table]
        A_WRK[Outbox Worker]
    end

    subgraph MessageBroker["RabbitMQ"]
        EXCHANGE[Exchange<br/>topic]
        QUEUE[Queue<br/>order.created]
    end

    subgraph ServiceB["Inventory Service"]
        B_CONS[Event Consumer]
        B_HANDLER[Business Logic]
        B_DB[(Inventory DB)]
    end

    A_ENT -->|Raises| A_DOM
    A_DOM -->|Processed by| A_CTX
    A_CTX -->|Writes to| A_OUT
    A_WRK -->|Polls| A_OUT
    A_WRK -->|Publishes| EXCHANGE
    EXCHANGE -->|Routes to| QUEUE
    QUEUE -->|Delivers| B_CONS
    B_CONS -->|Invokes| B_HANDLER
    B_HANDLER -->|Updates| B_DB

    style A_OUT fill:#ffccbc
    style EXCHANGE fill:#fff9c4
    style QUEUE fill:#fff9c4
```

### Outbox Pattern Details

**Why Outbox Pattern?**
- Ensures **atomic** write to database + message queue
- Prevents message loss if MQ is down
- Enables **at-least-once** delivery semantics
- Allows retry logic for failed messages

**Outbox Message Schema:**

```csharp
public class OutboxMessage
{
    public Guid Id { get; set; }
    public string EventType { get; set; } // "OrderCreatedEvent"
    public string Payload { get; set; }   // JSON serialized event
    public DateTime CreatedAt { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int Retries { get; set; }
}
```

---

## Infrastructure Architecture

### Shared Infrastructure Projects

#### 1. Infrastructure.Persistence

**Purpose**: Base persistence functionality for all services

**Contains:**
- `BaseDbContext` - Soft delete, audit fields, outbox integration
- `BaseRepository<TEntity, TId>` - Generic CRUD operations
- `OutboxMessage` entity
- Common EF Core configurations

**Usage:**
```csharp
public class UserDbContext : BaseDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Applies soft delete filter
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
```

#### 2. Infrastructure.Messaging

**Purpose**: MassTransit and RabbitMQ configuration

**Contains:**
- MassTransit setup
- RabbitMQ connection configuration
- Message serialization settings
- Retry policies

#### 3. Infrastructure.Exceptions

**Purpose**: Global exception handling

**Contains:**
- `GlobalExceptionHandler` middleware
- `DomainException` base class
- `NotFoundException`, `ValidationException`
- Problem Details factory

#### 4. Shared.Domain

**Purpose**: Core domain building blocks

**Contains:**
- `BaseEntity<TId>` - Entity base class
- `GuidEntity`, `LongEntity` - Convenience aliases
- `IDomainEvent` - Domain event marker interface
- `IBaseRepository<TEntity, TId>` - Repository contract
- `IUnitOfWork` - Unit of work pattern

### Service Discovery with Aspire

```mermaid
graph TB
    subgraph Aspire["Aspire AppHost"]
        SD[Service Discovery]
        CONFIG[Configuration]
        ORCH[Orchestration]
    end

    subgraph Services["Services"]
        US[User Service<br/>:5001]
        OS[Order Service<br/>:5002]
        IS[Inventory Service<br/>:5003]
    end

    subgraph Resources["Resources"]
        PG[PostgreSQL<br/>:5432]
        RMQ[RabbitMQ<br/>:5672]
        REDIS[Redis<br/>:6379]
    end

    ORCH -->|Registers| US & OS & IS
    ORCH -->|Provisions| PG & RMQ & REDIS
    SD -->|Resolves| US & OS & IS
    CONFIG -->|Injects| US & OS & IS

    US & OS & IS -->|Query| SD
```

---

## Deployment Architecture

### Local Development (Aspire)

```mermaid
graph TB
    subgraph Developer["Developer Machine"]
        subgraph Aspire["Aspire Dashboard :15888"]
            DASH[Dashboard UI]
            LOGS[Distributed Logs]
            TRACES[Distributed Traces]
            METRICS[Metrics]
        end

        subgraph Apps["Applications"]
            WEBAPP[WebApp :5000]
            GW[API Gateway :7000]
            US[User Service :5001]
            OS[Order Service :5002]
            IS[Inventory Service :5003]
        end

        subgraph Docker["Docker Containers"]
            PG_U[(PostgreSQL<br/>userservice)]
            PG_O[(PostgreSQL<br/>orderservice)]
            PG_I[(PostgreSQL<br/>inventoryservice)]
            RABBIT[RabbitMQ<br/>:5672, :15672]
            REDIS[Redis<br/>:6379]
        end
    end

    WEBAPP & GW & US & OS & IS -->|Telemetry| DASH
    WEBAPP & GW & US & OS & IS -->|Logs| LOGS
    WEBAPP & GW & US & OS & IS -->|Traces| TRACES
    WEBAPP & GW & US & OS & IS -->|Metrics| METRICS

    US --> PG_U
    OS --> PG_O
    IS --> PG_I
    US & OS & IS --> RABBIT
    GW --> REDIS
```

### Container Architecture (Future)

```mermaid
graph TB
    subgraph Cluster["Kubernetes Cluster"]
        subgraph Ingress["Ingress Layer"]
            ING[NGINX Ingress<br/>Load Balancer]
        end

        subgraph Services["Service Pods"]
            US_POD1[User Service<br/>Pod 1]
            US_POD2[User Service<br/>Pod 2]
            OS_POD1[Order Service<br/>Pod 1]
            IS_POD1[Inventory Service<br/>Pod 1]
        end

        subgraph Data["Data Layer"]
            PG_US[(PostgreSQL<br/>StatefulSet)]
            PG_OS[(PostgreSQL<br/>StatefulSet)]
            PG_IS[(PostgreSQL<br/>StatefulSet)]
            RABBIT_K8S[RabbitMQ<br/>StatefulSet]
            REDIS_K8S[Redis<br/>StatefulSet]
        end
    end

    ING --> US_POD1 & US_POD2
    ING --> OS_POD1
    ING --> IS_POD1

    US_POD1 & US_POD2 --> PG_US
    OS_POD1 --> PG_OS
    IS_POD1 --> PG_IS
    US_POD1 & US_POD2 & OS_POD1 & IS_POD1 --> RABBIT_K8S
    ING --> REDIS_K8S
```

---

## Scalability Considerations

### Horizontal Scaling

```mermaid
graph LR
    subgraph LB["Load Balancer"]
        NGINX[NGINX]
    end

    subgraph Services["User Service Instances"]
        US1[Instance 1<br/>Pod 1]
        US2[Instance 2<br/>Pod 2]
        US3[Instance 3<br/>Pod 3]
    end

    subgraph Data["Shared Resources"]
        DB[(PostgreSQL<br/>Primary + Replicas)]
        CACHE[(Redis Cluster)]
        MQ[RabbitMQ Cluster]
    end

    NGINX -->|Round Robin| US1
    NGINX -->|Round Robin| US2
    NGINX -->|Round Robin| US3

    US1 & US2 & US3 --> DB
    US1 & US2 & US3 --> CACHE
    US1 & US2 & US3 --> MQ
```

### Database Scaling

**Read Replicas:**
- Use PostgreSQL read replicas for read-heavy workloads
- CQRS pattern to separate read/write models

**Sharding (Future):**
- Shard by tenant ID or user ID
- Implement routing logic in repositories

**Caching Strategy:**
- L1 Cache: In-memory per service instance
- L2 Cache: Redis shared cache
- Cache invalidation via events

---

## Security Architecture

### Authentication Flow

```mermaid
sequenceDiagram
    participant Client
    participant Gateway
    participant UserService
    participant JWT as JWT Service
    participant DB as User DB

    Client->>Gateway: POST /auth/login<br/>{email, password}
    Gateway->>UserService: Forward request
    UserService->>DB: Verify credentials
    DB-->>UserService: User found
    UserService->>JWT: Generate JWT token
    JWT-->>UserService: Access Token + Refresh Token
    UserService-->>Gateway: 200 OK + tokens
    Gateway-->>Client: tokens

    Client->>Gateway: GET /users/me<br/>Authorization: Bearer {token}
    Gateway->>Gateway: Validate JWT
    Gateway->>UserService: Forward with user claims
    UserService-->>Gateway: User data
    Gateway-->>Client: User data
```

### Authorization Levels

1. **Gateway Level**: Rate limiting, IP filtering
2. **Service Level**: Role-based access control (RBAC)
3. **Domain Level**: Business rule validation

---

## Monitoring & Observability

### OpenTelemetry Integration

```mermaid
graph TB
    subgraph Services["Microservices"]
        US[User Service]
        OS[Order Service]
        IS[Inventory Service]
    end

    subgraph Telemetry["OpenTelemetry"]
        LOGS[Logs]
        TRACES[Distributed Traces]
        METRICS[Metrics]
    end

    subgraph Backends["Observability Backends"]
        JAEGER[Jaeger<br/>Traces]
        PROM[Prometheus<br/>Metrics]
        LOKI[Loki<br/>Logs]
    end

    US & OS & IS -->|OTLP| LOGS
    US & OS & IS -->|OTLP| TRACES
    US & OS & IS -->|OTLP| METRICS

    LOGS --> LOKI
    TRACES --> JAEGER
    METRICS --> PROM
```

---

## Conclusion

This architecture provides:
- ✅ **Scalability** through microservices and horizontal scaling
- ✅ **Resilience** via outbox pattern and eventual consistency
- ✅ **Maintainability** through clean architecture and DDD
- ✅ **Observability** with OpenTelemetry and Aspire
- ✅ **Flexibility** to evolve services independently

For implementation details, refer to the code in the respective projects.
