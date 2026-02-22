using Microsoft.AspNetCore.Mvc;
using OrderService.Contracts.Requests;
using OrderService.Contracts.Responses;
using OrderService.Domain.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _orderService.GetAllAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpGet("number/{orderNumber}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetByOrderNumber(string orderNumber, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByOrderNumberAsync(orderNumber, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetByCustomerId(Guid customerId, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByCustomerIdAsync(customerId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _orderService.CreateOrderAsync(request, cancellationToken);
        
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);

        return HandleError(result);
    }

    [HttpPost("{orderId:guid}/items")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> AddItem(Guid orderId, [FromBody] AddOrderItemRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _orderService.AddOrderItemAsync(orderId, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpDelete("{orderId:guid}/items/{itemId:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> RemoveItem(Guid orderId, Guid itemId, CancellationToken cancellationToken)
    {
        var result = await _orderService.RemoveOrderItemAsync(orderId, itemId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpPost("{orderId:guid}/confirm")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> Confirm(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _orderService.ConfirmOrderAsync(orderId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpPost("{orderId:guid}/ship")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> Ship(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _orderService.ShipOrderAsync(orderId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpPost("{orderId:guid}/deliver")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> Deliver(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _orderService.DeliverOrderAsync(orderId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpPost("{orderId:guid}/cancel")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> Cancel(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _orderService.CancelOrderAsync(orderId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _orderService.DeleteOrderAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleError(result);
    }

    private ActionResult HandleError<T>(Shared.Common.Results.Result<T> result)
    {
        var error = result.Error!;
        return error.Code switch
        {
            "Order.NotFound" => NotFound(new { error.Code, error.Message }),
            "Order.InvalidOperation" => BadRequest(new { error.Code, error.Message }),
            "Order.DuplicateOrderNumber" => Conflict(new { error.Code, error.Message }),
            _ => StatusCode(500, new { error.Code, error.Message })
        };
    }

    private ActionResult HandleError(Shared.Common.Results.Result result)
    {
        var error = result.Error!;
        return error.Code switch
        {
            "Order.NotFound" => NotFound(new { error.Code, error.Message }),
            "Order.InvalidOperation" => BadRequest(new { error.Code, error.Message }),
            "Order.DuplicateOrderNumber" => Conflict(new { error.Code, error.Message }),
            _ => StatusCode(500, new { error.Code, error.Message })
        };
    }
}
