using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure;
using OrderService.Infrastructure.DataSeeders;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

// Add OpenAPI and Scalar UI support
builder.Services.AddOpenApi();

var app = builder.Build();

// Apply migrations and seed data in development
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Ensuring OrderService database exists...");

            // Check if database can be connected to
            var canConnect = await dbContext.Database.CanConnectAsync();
            if (!canConnect)
            {
                logger.LogWarning("Database does not exist, creating...");
                await dbContext.Database.EnsureCreatedAsync();
            }
            else
            {
                logger.LogInformation("Applying OrderService database migrations...");
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("OrderService database migrations applied successfully");
            }

            logger.LogInformation("Seeding OrderService database...");
            await app.Services.SeedOrderDatabaseAsync();
            logger.LogInformation("OrderService database seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating or seeding the database");
            throw;
        }
    }
}

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Order Service API";
        options.Theme = ScalarTheme.Purple;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
