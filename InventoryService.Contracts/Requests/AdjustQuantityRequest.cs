using System.ComponentModel.DataAnnotations;

namespace InventoryService.Contracts.Requests;

public class AdjustQuantityRequest
{
    [Required]
    public int Adjustment { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Reason { get; set; } = string.Empty;
}
