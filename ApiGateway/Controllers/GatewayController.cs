using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

/// <summary>
/// Gateway status and information endpoint
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GatewayController : ControllerBase
{
    /// <summary>
    /// Get API Gateway status
    /// </summary>
    /// <returns>Gateway status information</returns>
    [HttpGet("status")]
    [ProducesResponseType(typeof(GatewayStatus), StatusCodes.Status200OK)]
    public IActionResult GetStatus()
    {
        return Ok(new GatewayStatus
        {
            Name = "API Gateway",
            Version = "1.0.0",
            Status = "Running",
            Timestamp = DateTime.UtcNow,
            AvailableServices = new List<string>
            {
                "UserService - /api/users/*",
                "OrderService - /api/orders/*",
                "InventoryService - /api/inventory/*"
            }
        });
    }

    /// <summary>
    /// Get information about available routes
    /// </summary>
    /// <returns>List of available service routes</returns>
    [HttpGet("routes")]
    [ProducesResponseType(typeof(RouteInfo), StatusCodes.Status200OK)]
    public IActionResult GetRoutes()
    {
        return Ok(new RouteInfo
        {
            Routes = new List<RouteDetail>
            {
                new RouteDetail
                {
                    Service = "UserService",
                    PathPrefix = "/api/users",
                    Description = "User management and authentication",
                    ScalarUrl = "/api/users/scalar/v1"
                },
                new RouteDetail
                {
                    Service = "OrderService",
                    PathPrefix = "/api/orders",
                    Description = "Order management and processing",
                    ScalarUrl = "/api/orders/scalar/v1"
                },
                new RouteDetail
                {
                    Service = "InventoryService",
                    PathPrefix = "/api/inventory",
                    Description = "Product inventory management",
                    ScalarUrl = "/api/inventory/scalar/v1"
                }
            }
        });
    }
}

public record GatewayStatus
{
    public string Name { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public List<string> AvailableServices { get; init; } = new();
}

public record RouteInfo
{
    public List<RouteDetail> Routes { get; init; } = new();
}

public record RouteDetail
{
    public string Service { get; init; } = string.Empty;
    public string PathPrefix { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ScalarUrl { get; init; } = string.Empty;
}
