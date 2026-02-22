using System.ComponentModel.DataAnnotations;

namespace OrderService.Contracts.Requests;

public class CreateOrderRequest
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string OrderNumber { get; set; } = string.Empty;
}
