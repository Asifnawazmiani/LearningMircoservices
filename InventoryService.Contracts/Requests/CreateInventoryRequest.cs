using System.ComponentModel.DataAnnotations;

namespace InventoryService.Contracts.Requests;

public class CreateInventoryRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue)]
    public int InitialQuantity { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int ReorderQuantity { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Location { get; set; } = string.Empty;
}
