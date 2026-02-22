using System.ComponentModel.DataAnnotations;

namespace InventoryService.Contracts.Requests;

public class UpdateReorderSettingsRequest
{
    [Required]
    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int ReorderQuantity { get; set; }
}
