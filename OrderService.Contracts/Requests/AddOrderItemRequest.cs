using System.ComponentModel.DataAnnotations;

namespace OrderService.Contracts.Requests;

public class AddOrderItemRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero")]
    public decimal UnitPrice { get; set; }
}
