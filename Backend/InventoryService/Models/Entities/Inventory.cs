namespace InventoryService.Models.Entities;

public class Inventory
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int StockQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public string WarehouseLocation { get; set; } = "Main";
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public int AvailableQuantity => StockQuantity - ReservedQuantity;
    public Product Product { get; set; } = null!;
}