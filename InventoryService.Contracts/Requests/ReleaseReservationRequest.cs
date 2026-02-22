using System.ComponentModel.DataAnnotations;

namespace InventoryService.Contracts.Requests;

public class ReleaseReservationRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
