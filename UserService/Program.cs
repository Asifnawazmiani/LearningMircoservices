using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure;
using UserService.Infrastructure.DataSeeders;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add Infrastructure layer services (Repositories, DbContext, Domain Services)
builder.Services.AddUserInfrastructure(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddExceptionHandling();

// Add OpenAPI and Scalar UI support
builder.Services.AddOpenApi();

var app = builder.Build();

// Apply migrations and seed data in development
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Ensuring UserService database exists...");

            // Check if database can be connected to
            var canConnect = await dbContext.Database.CanConnectAsync();
            if (!canConnect)
            {
                logger.LogWarning("Database does not exist, creating...");
                await dbContext.Database.EnsureCreatedAsync();
            }
            else
            {
                logger.LogInformation("Applying UserService database migrations...");
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("UserService database migrations applied successfully");
            }

            logger.LogInformation("Seeding UserService database...");
            await app.Services.SeedUserDatabaseAsync();
            logger.LogInformation("UserService database seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating or seeding the database");
            throw;
        }
    }
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "User Service API";
        options.Theme = ScalarTheme.Purple;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Use exception handling
app.UseExceptionHandling();

app.Run();
