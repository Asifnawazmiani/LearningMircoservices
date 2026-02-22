using InventoryService.Contracts.Requests;
using InventoryService.Contracts.Responses;
using InventoryService.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductInventoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductInventoryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetAllAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductInventoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductInventoryResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpGet("product/{productId:guid}")]
    [ProducesResponseType(typeof(ProductInventoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductInventoryResponse>> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetByProductIdAsync(productId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpGet("sku/{sku}")]
    [ProducesResponseType(typeof(ProductInventoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductInventoryResponse>> GetBySku(string sku, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetBySkuAsync(sku, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(IEnumerable<ProductInventoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductInventoryResponse>>> GetLowStockItems(CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetLowStockItemsAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductInventoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductInventoryResponse>> Create([FromBody] CreateInventoryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _inventoryService.CreateInventoryAsync(request, cancellationToken);
        
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);

        return HandleError(result);
    }

    [HttpPost("{id:guid}/adjust")]
    [ProducesResponseType(typeof(ProductInventoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductInventoryResponse>> AdjustQuantity(Guid id, [FromBody] AdjustQuantityRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _inventoryService.AdjustQuantityAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpPost("{id:guid}/reserve")]
    [ProducesResponseType(typeof(ProductInventoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductInventoryResponse>> ReserveQuantity(Guid id, [FromBody] ReserveQuantityRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _inventoryService.ReserveQuantityAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpPost("{id:guid}/release")]
    [ProducesResponseType(typeof(ProductInventoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductInventoryResponse>> ReleaseReservation(Guid id, [FromBody] ReleaseReservationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _inventoryService.ReleaseReservationAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpPost("{id:guid}/fulfill")]
    [ProducesResponseType(typeof(ProductInventoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductInventoryResponse>> FulfillReservation(Guid id, [FromBody] FulfillReservationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _inventoryService.FulfillReservationAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpPut("{id:guid}/reorder-settings")]
    [ProducesResponseType(typeof(ProductInventoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductInventoryResponse>> UpdateReorderSettings(Guid id, [FromBody] UpdateReorderSettingsRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _inventoryService.UpdateReorderSettingsAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.DeleteInventoryAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleError(result);
    }

    private ActionResult HandleError<T>(Shared.Common.Results.Result<T> result)
    {
        var error = result.Error!;
        return error.Code switch
        {
            "Inventory.NotFound" => NotFound(new { error.Code, error.Message }),
            "Inventory.InvalidOperation" or "Inventory.ValidationError" => BadRequest(new { error.Code, error.Message }),
            "Inventory.DuplicateProduct" or "Inventory.DuplicateSku" => Conflict(new { error.Code, error.Message }),
            _ => StatusCode(500, new { error.Code, error.Message })
        };
    }

    private ActionResult HandleError(Shared.Common.Results.Result result)
    {
        var error = result.Error!;
        return error.Code switch
        {
            "Inventory.NotFound" => NotFound(new { error.Code, error.Message }),
            "Inventory.InvalidOperation" or "Inventory.ValidationError" => BadRequest(new { error.Code, error.Message }),
            "Inventory.DuplicateProduct" or "Inventory.DuplicateSku" => Conflict(new { error.Code, error.Message }),
            _ => StatusCode(500, new { error.Code, error.Message })
        };
    }
}
