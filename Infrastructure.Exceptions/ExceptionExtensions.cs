using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Exceptions;

public static class ExceptionExtensions
{
    /// <summary>
    /// Call in Program.cs: builder.Services.AddExceptionHandling()
    /// </summary>
    public static IServiceCollection AddExceptionHandling(
        this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        return services;
    }

    /// <summary>
    /// Call in Program.cs: app.UseExceptionHandling()
    /// </summary>
    public static IApplicationBuilder UseExceptionHandling(
        this IApplicationBuilder app)
    {
        app.UseExceptionHandler();
        return app;
    }
}
