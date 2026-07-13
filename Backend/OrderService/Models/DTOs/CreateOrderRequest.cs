namespace OrderService.Models.DTOs;

public record CreateOrderRequest(string ProductName, int Quantity, decimal Amount);