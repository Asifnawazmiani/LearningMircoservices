using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken ct)
    {
        _logger.LogError(exception,
            "Exception occurred: {Message}", exception.Message);

        var (statusCode, problem) = MapException(context, exception);

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(problem, ct);

        return true;
    }

    private static (int StatusCode, ProblemDetails Problem) MapException(
        HttpContext context,
        Exception exception)
    {
        var correlationId = context.Items["CorrelationId"]?.ToString();
        var requestId = context.Items["RequestId"]?.ToString();

        ProblemDetails problem = exception switch
        {
            NotFoundException ex => new ProblemDetails
            {
                Title = "Not Found",
                Detail = ex.Message,
                Status = 404,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            },

            ValidationException ex => BuildValidationProblem(ex),

            DuplicateException ex => new ProblemDetails
            {
                Title = "Conflict",
                Detail = ex.Message,
                Status = 409,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            },

            ConflictException ex => new ProblemDetails
            {
                Title = "Conflict",
                Detail = ex.Message,
                Status = 409,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            },

            UnauthorizedException ex => new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = ex.Message,
                Status = 401,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            },

            ForbiddenException ex => new ProblemDetails
            {
                Title = "Forbidden",
                Detail = ex.Message,
                Status = 403,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
            },

            _ => new ProblemDetails
            {
                Title = "Server Error",
                Detail = "An unexpected error occurred",
                Status = 500,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            }
        };

        // Attach tracing info to every error
        problem.Extensions["correlationId"] = correlationId;
        problem.Extensions["requestId"] = requestId;
        problem.Extensions["timestamp"] = DateTime.UtcNow;

        return (problem.Status!.Value, problem);
    }

    private static ValidationProblemDetails BuildValidationProblem(
        ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => e.Field ?? string.Empty)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Message).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Title = "Validation Failed",
            Status = 422,
            Type = "https://tools.ietf.org/html/rfc4918#section-11.2"
        };
    }
}