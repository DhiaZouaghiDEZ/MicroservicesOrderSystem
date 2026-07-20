namespace InventoryService.Models.DTOs;

public class CreateProductRequest
{
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int StockQuantity { get; set; }
}

public class RestockRequest
{
    public int Quantity { get; set; }
}