using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Repositories;
using Shared.Domain.UnitOfWork;

namespace OrderService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("orders") 
            ?? throw new InvalidOperationException("orders connection string not found");

        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<OrderDbContext>());
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<Domain.Services.IOrderService, Services.OrderService>();

        return services;
    }
}
