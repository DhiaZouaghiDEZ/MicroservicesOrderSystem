using InventoryService.Models.DTOs;
using InventoryService.Models.Entities;

namespace InventoryService.Services;

public interface IInventoryService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<Product> CreateProductAsync(CreateProductRequest request);
    Task<Inventory?> GetInventoryByProductIdAsync(Guid productId);
    Task RestockAsync(Guid productId, int quantity);
    Task DeleteProductAsync(Guid id);
}