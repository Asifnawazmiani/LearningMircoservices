using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.UnitOfWork;
using UserService.Domain.Repositories;
using UserService.Domain.Services;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddUserInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("users") 
            ?? throw new InvalidOperationException("users connection string not found");

        // Register DbContext
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register DbContext as UnitOfWork
        services.AddScoped<IUnitOfWork>(provider => 
            provider.GetRequiredService<UserDbContext>());

        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Register Domain Services
        services.AddScoped<IUserService, Services.UserService>();

        return services;
    }
}
