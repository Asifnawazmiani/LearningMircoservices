var builder = DistributedApplication.CreateBuilder(args);

// ══════════════════════════════════════
// RabbitMQ — Shared across all services
// ══════════════════════════════════════
var rabbitmq = builder.AddRabbitMQ("rabbitmq",
    userName: builder.AddParameter("rabbitmq-user", value: "admin", secret: false),
    password: builder.AddParameter("rabbitmq-password", secret: true))
    .WithManagementPlugin()          // UI at http://localhost:15672
    .WithDataVolume("rabbitmq-data") // Persist messages
    .WithLifetime(ContainerLifetime.Persistent); // Keep running between restarts

// ══════════════════════════════════════
// PostgreSQL — Separate DB per service
// ══════════════════════════════════════

// Order Service DB
var orderPostgres = builder.AddPostgres("orders-postgres")
    .WithDataVolume("order-postgres-data")
    .WithPgAdmin()                   // PgAdmin UI for order DB
    .WithLifetime(ContainerLifetime.Persistent);

var orderDb = orderPostgres.AddDatabase("orders");

// Inventory Service DB
var inventoryPostgres = builder.AddPostgres("inventory-postgres")
    .WithDataVolume("inventory-postgres-data")
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

var inventoryDb = inventoryPostgres.AddDatabase("inventory");

// User Service DB
var userPostgres = builder.AddPostgres("user-postgres")
    .WithDataVolume("user-postgres-data")
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

var userDb = userPostgres.AddDatabase("users");

// ══════════════════════════════════════
// Redis — Separate cache per service
// ══════════════════════════════════════

// Order Service Cache
var orderRedis = builder.AddRedis("order-redis")
    .WithDataVolume("order-redis-data")
    .WithRedisInsight()              // RedisInsight UI
    .WithLifetime(ContainerLifetime.Persistent);

// Inventory Service Cache
var inventoryRedis = builder.AddRedis("inventory-redis")
    .WithDataVolume("inventory-redis-data")
    .WithRedisInsight()
    .WithLifetime(ContainerLifetime.Persistent);

// User Service Cache
var userRedis = builder.AddRedis("user-redis")
    .WithDataVolume("user-redis-data")
    .WithRedisInsight()
    .WithLifetime(ContainerLifetime.Persistent);

// ══════════════════════════════════════
// Services
// ══════════════════════════════════════
var userService = builder.AddProject<Projects.UserService>("user-service")
    .WithReference(userDb)
    .WithReference(userRedis)
    .WithReference(rabbitmq)
    .WaitFor(userPostgres)
    .WaitFor(userRedis)
    .WaitFor(rabbitmq);

var inventoryService = builder.AddProject<Projects.InventoryService>("inventory-service")
    .WithReference(inventoryDb)
    .WithReference(inventoryRedis)
    .WithReference(rabbitmq)
    .WaitFor(inventoryPostgres)
    .WaitFor(inventoryRedis)
    .WaitFor(rabbitmq);

var orderService = builder.AddProject<Projects.OrderService>("order-service")
    .WithReference(orderDb)
    .WithReference(orderRedis)
    .WithReference(rabbitmq)
    .WithReference(inventoryService)
    .WithReference(userService)
    .WaitFor(orderPostgres)
    .WaitFor(orderRedis)
    .WaitFor(rabbitmq)
    .WaitFor(inventoryService)
    .WaitFor(userService);

builder.AddProject<Projects.ApiGateway>("api-gateway")
    .WithReference(orderService)
    .WithReference(inventoryService)
    .WithReference(userService)
    .WaitFor(orderService)
    .WaitFor(inventoryService)
    .WaitFor(userService)
    .WithExternalHttpEndpoints();


builder.AddProject<Projects.WebApp>("webapp");


builder.Build().Run();
