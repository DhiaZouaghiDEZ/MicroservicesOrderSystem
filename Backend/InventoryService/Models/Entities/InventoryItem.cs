namespace InventoryService.Models.Entities;

public class InventoryItem
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
}