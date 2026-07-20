using Microsoft.EntityFrameworkCore;
using InventoryService.Data;
using InventoryService.Models.DTOs;
using InventoryService.Models.Entities;

namespace InventoryService.Services;

public class InventoryService : IInventoryService
{
    private readonly InventoryDbContext _context;

    public InventoryService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products
            .Include(p => p.Inventory)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateProductAsync(CreateProductRequest request)
    {
        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductName.ToLower() == request.ProductName.ToLower().Trim());

        if (existingProduct != null)
        {
            throw new InvalidOperationException($"A product with the name '{request.ProductName}' already exists.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            ProductName = request.ProductName.Trim(),
            Price = request.Price,
            Description = request.Description.Trim(),
            ImageUrl = request.ImageUrl.Trim(),
            Category = request.Category?.Trim() ?? "General"
        };

        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            StockQuantity = request.StockQuantity,
            ReservedQuantity = 0,
            WarehouseLocation = "Main"
        };

        product.Inventory = inventory;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<Inventory?> GetInventoryByProductIdAsync(Guid productId)
    {
        return await _context.Inventory
            .FirstOrDefaultAsync(i => i.ProductId == productId);
    }

    public async Task RestockAsync(Guid productId, int quantity)
    {
        var inventory = await _context.Inventory
            .FirstOrDefaultAsync(i => i.ProductId == productId);

        if (inventory != null)
        {
            inventory.StockQuantity += quantity;
            inventory.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}