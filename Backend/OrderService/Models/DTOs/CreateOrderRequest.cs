namespace OrderService.Models.DTOs;

public class CreateOrderRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string CardNumber { get; set; } = string.Empty;
}