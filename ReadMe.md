## Introduction
This repository is example implementation of microservices architecture and design patterns. It uses mix architecture called "Microservices with Clean Architecture and DDD" or "Distributed Clean Architecture", more inspired by [eShopOnContainers Architecture](https://github.com/dotnet/eShop/tree/main/src).
This purpoose of writing this repo is not build a functional application, but to show how to implement microservices architecture and design patterns in a simple way. It is not a production ready code, but it can be used as a reference for learning and understanding the concepts.

## Project Structure
The solution is organized into multiple projects, each representing a different layer or component of the architecture.

```
src/
│
├── AppHost/                          ← .NET Aspire orchestration
│
├── Gateway/
│   └── ApiGateway/                   ← YARP reverse proxy
│
├── App/
│   └── WebApp/                       ← Frontend client
│
├── Services/
│   ├── UserService/                  ← ASP.NET Core API
│   ├── OrderService/
│   └── InventoryService/
│
├── Domain/
│   ├── UserService.Domain/           ← Entities, VOs, Events, Exceptions
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Events/
│   │   └── Exceptions/
│   ├── OrderService.Domain/
│   └── InventoryService.Domain/
│
├── Infrastructure/
│   ├── Infrastructure.Persistence/   ← Shared base (BaseDbContext, GuidEntity, Outbox)
│   │   ├── BaseDbContext.cs
│   │   ├── Entities/                 ← BaseEntity, GuidEntity, LongEntity
│   │   └── Outbox/                   ← OutboxMessage
│   │
│   ├── Infrastructure.Messaging/     ← Shared messaging (MassTransit / RabbitMQ)
│   │
│   ├── UserService.Infrastructure/   ← User-specific EF Core wiring
│   │   ├── UserDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── UserConfiguration.cs
│   │   │   └── UserRoleConfiguration.cs
│   │   └── Migrations/
│   │
│   ├── OrderService.Infrastructure/
│   │   ├── OrderDbContext.cs
│   │   ├── Configurations/
│   │   └── Migrations/
│   │
│   └── InventoryService.Infrastructure/
│       ├── InventoryDbContext.cs
│       ├── Configurations/
│       └── Migrations/
│
├── Clients/
│   ├── UserService.Client/           ← Typed HTTP SDK
│   ├── OrderService.Client/
│   └── InventoryService.Client/
│
└── Shared/
    ├── Shared.Common/                ← Cross-cutting concerns
    ├── Shared.Events/                ← Integration event contracts
    └── ServiceDefaults/              ← Aspire defaults, OpenTelemetry
```


## It's architecture contains:
| Pattern | Where in your solution |
|---------|------------------------|
|Clean Architecture    |    Layer separation|
|DDD                   |    Domain folder (entities, VO, exceptions)|
|Microservices         |    Services folder|
|Outbox Pattern        |    Infrastructure.Persistence/Outbox|
|Client SDK Pattern    |    Clients folder|
|Shared Kernel         |    Shared folder|
|API Gateway           |    Gateway folder|
|Modular Monorepo      |    Single solution, multiple projects|

## Microservices
- **User Service**: Manages users of the system.
- **Order Service**: Manages orders and their lifecycle.
- **Inventory Service**: Manages inventory and stock levels.

---

## Devloper Guide Lines:
   - The solution is monorepo microservices architecture, where all services and shared libraries are in a single solution for easier development and code sharing.
   - The solution has central package management for consistent dependency versions across projects.
   - Each service has its own database and is responsible for its own data management, following the database per service pattern.
   - Each service is an independent ASP.NET Core Web API project.
   - The domain layer is separated into its own project for each service, containing entities, value objects, domain events, and exceptions.
   - The auto generated ids are are guide version 7 for better performance and scalability. As it uses time-based UUIDs, it provides better indexing and query performance compared to random UUIDs, especially in high-concurrency scenarios.

## Solution Diagrams

### High-Level Architecture

```mermaid
graph TB
    subgraph ORCH["⚙️ Aspire Orchestration — AppHost"]
        AppHost["AppHost\nAspire.AppHost\nPostgreSQL · RabbitMQ · Redis"]
    end

    subgraph ENTRY["Entry Points"]
        WebApp["🌐 WebApp"]
        GW["🔀 ApiGateway\nRouting / Auth"]
    end

    subgraph US["👤 User Service"]
        direction TB
        US_API["UserService\nASP.NET Core API"]
        US_DOM["UserService.Domain\nEntities · Value Objects\nDomain Events · Exceptions"]
        US_INF["UserService.Infrastructure\nUserDbContext · EF Configs\nMigrations"]
        US_CLI["UserService.Client\nHTTP SDK"]
        US_API --> US_DOM
        US_INF --> US_DOM
    end

    subgraph OS["📦 Order Service"]
        direction TB
        OS_API["OrderService\nASP.NET Core API"]
        OS_DOM["OrderService.Domain\nEntities · Value Objects\nDomain Events · Exceptions"]
        OS_INF["OrderService.Infrastructure\nOrderDbContext · EF Configs\nMigrations"]
        OS_CLI["OrderService.Client\nHTTP SDK"]
        OS_API --> OS_DOM
        OS_INF --> OS_DOM
    end

    subgraph IS["🏭 Inventory Service"]
        direction TB
        IS_API["InventoryService\nASP.NET Core API"]
        IS_DOM["InventoryService.Domain\nEntities · Value Objects\nDomain Events · Exceptions"]
        IS_INF["InventoryService.Infrastructure\nInventoryDbContext · EF Configs\nMigrations"]
        IS_CLI["InventoryService.Client\nHTTP SDK"]
        IS_API --> IS_DOM
        IS_INF --> IS_DOM
    end

    subgraph INFRA["🔧 Shared Infrastructure"]
        IP["Infrastructure.Persistence\nBaseDbContext · GuidEntity\nOutboxMessage · Soft Delete"]
        IM["Infrastructure.Messaging\nMassTransit · RabbitMQ"]
        SD["ServiceDefaults\nAspire Defaults · OpenTelemetry\nHealth Checks · Service Discovery"]
    end

    subgraph KERNEL["📚 Shared Kernel"]
        SE["Shared.Events\nIntegration Event Contracts"]
        SC["Shared.Common\nCross-cutting Concerns"]
    end

    subgraph EXT["🗄️ External Services"]
        PG[("PostgreSQL\nper-service DB")]
        RMQ[("RabbitMQ\nMessage Broker")]
        RD[("Redis\nCache / Sessions")]
    end

    AppHost -.->|orchestrates| WebApp & GW & US_API & OS_API & IS_API

    WebApp -->|HTTPS| GW
    GW -->|route /users| US_API
    GW -->|route /orders| OS_API
    GW -->|route /inventory| IS_API

    US_API & OS_API & IS_API --> SD
    US_DOM & OS_DOM & IS_DOM --> IP
    US_INF & OS_INF & IS_INF --> IP

    US_API & OS_API & IS_API -->|Npgsql| PG
    US_API & OS_API & IS_API -->|AMQP| RMQ
    GW -->|cache| RD
```

---

### Project Dependency Graph

```mermaid
graph LR
    subgraph HOST["Orchestration"]
        AppHost
    end

    subgraph APIS["Service APIs"]
        UserService
        OrderService
        InventoryService
        ApiGateway
        WebApp
    end

    subgraph DOMAINS["Domain Projects"]
        USD["UserService.Domain"]
        OSD["OrderService.Domain"]
        ISD["InventoryService.Domain"]
    end

    subgraph INFS["Infrastructure Projects"]
        USI["UserService.Infrastructure"]
        OSI["OrderService.Infrastructure"]
        ISI["InventoryService.Infrastructure"]
    end

    subgraph CLIENTS["Client SDKs"]
        USC["UserService.Client"]
        OSC["OrderService.Client"]
        ISC["InventoryService.Client"]
    end

    subgraph SHARED["Shared"]
        IP["Infrastructure.Persistence"]
        IM["Infrastructure.Messaging"]
        SD["ServiceDefaults"]
        SE["Shared.Events"]
        SC["Shared.Common"]
    end

    AppHost --> UserService & OrderService & InventoryService & ApiGateway & WebApp

    UserService --> USD & SD
    OrderService --> OSD & SD
    InventoryService --> ISD & SD
    ApiGateway --> SD

    USD --> IP
    OSD --> IP
    ISD --> IP

    USI --> USD & IP
    OSI --> OSD & IP
    ISI --> ISD & IP
```

---

### Clean Architecture Layers *(UserService — fully implemented)*

```mermaid
graph TB
    subgraph API["API Layer — UserService"]
        CTRL["Controllers"]
        PRG["Program.cs\nDI Registration"]
    end

    subgraph DOM["Domain Layer — UserService.Domain"]
        subgraph ENT["Entities"]
            UE["UserEntity\n+ GuidEntity"]
            URE["UserRoleEntity"]
        end
        subgraph VO["Value Objects"]
            EM["Email"]
            FN["FullName"]
            MN["MobileNo"]
        end
        subgraph EVT["Domain Events"]
            IDE["IDomainEvent"]
            UDE["UserCreatedEvent\nUserEmailVerifiedEvent\nUserSuspendedEvent\n+ 7 more"]
        end
        subgraph EXC["Exceptions"]
            DE["DomainException"]
            NF["NotFoundException"]
            UX["UserDomainException\nUserNotFoundException\nUserSuspendedException\n..."]
        end
    end

    subgraph INF["Infrastructure Layer — UserService.Infrastructure"]
        DBC["UserDbContext\n↳ BaseDbContext\n  ↳ DbContext"]
        CFG["EF Configurations\nUserConfiguration\nUserRoleConfiguration"]
        MIG["Migrations"]
    end

    subgraph PERSIST["Infrastructure.Persistence"]
        BDB["BaseDbContext\nOutboxMessages DbSet\nSoft Delete Filter\nAudit Fields"]
        BE["GuidEntity / LongEntity\nId · CreatedAt · UpdatedAt\nIsDeleted · DeletedAt"]
        OBX["OutboxMessage\nEventType · Payload\nRetries · Dead Letter"]
    end

    API --> DOM
    INF --> DOM
    INF --> PERSIST
    DOM --> PERSIST

    DBC -->|SaveChangesAsync\ndrains domain events| OBX
    UE -->|raises| UDE
    UE -->|inherits| BE
    DBC -->|inherits| BDB
```

---

### Outbox Pattern Flow

```mermaid
sequenceDiagram
    participant C as Controller
    participant E as UserEntity
    participant DB as UserDbContext
    participant OBX as OutboxMessages
    participant PG as PostgreSQL
    participant W as Outbox Worker
    participant MQ as RabbitMQ

    C->>E: entity.Suspend(reason)
    E->>E: Raise(UserSuspendedEvent)

    C->>DB: SaveChangesAsync()
    DB->>E: collect DomainEvents
    DB->>OBX: Add OutboxMessage(EventType, JSON payload)
    E->>E: ClearDomainEvents()
    DB->>PG: Single transaction commit<br/>(entity row + outbox row)

    W->>PG: Poll unpublished OutboxMessages
    W->>MQ: Publish message
    W->>PG: Mark IsPublished = true
    MQ-->>C: Other services consume event
```
