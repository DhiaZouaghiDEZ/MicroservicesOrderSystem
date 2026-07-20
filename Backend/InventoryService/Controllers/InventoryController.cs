using Microsoft.AspNetCore.Mvc;
using InventoryService.Models.DTOs;
using InventoryService.Services;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _inventoryService.GetAllProductsAsync();

        var result = products.Select(p => new
        {
            p.Id,
            p.ProductName,
            p.Price,
            p.Description,
            p.ImageUrl,
            p.Category,
            StockQuantity = p.Inventory?.StockQuantity ?? 0,
            ReservedQuantity = p.Inventory?.ReservedQuantity ?? 0,
            AvailableQuantity = p.Inventory?.AvailableQuantity ?? 0,
            IsAvailable = (p.Inventory?.AvailableQuantity ?? 0) > 0
        });

        return Ok(result);
    }

    [HttpGet("products/{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await _inventoryService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound(new { Message = "Product not found" });

        return Ok(product);
    }

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            var product = await _inventoryService.CreateProductAsync(request);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "An error occurred while creating the product." });
        }
    }

    [HttpPost("products/{id}/restock")]
    public async Task<IActionResult> Restock(Guid id, [FromBody] RestockRequest request)
    {
        var inventory = await _inventoryService.GetInventoryByProductIdAsync(id);
        if (inventory == null)
            return NotFound(new { Message = "Product not found" });

        await _inventoryService.RestockAsync(id, request.Quantity);

        return Ok(new
        {
            Message = "Stock added successfully",
            NewStockQuantity = inventory.StockQuantity + request.Quantity
        });
    }

    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var product = await _inventoryService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound(new { Message = "Product not found" });

        await _inventoryService.DeleteProductAsync(id);
        return Ok(new { Message = "Product deleted successfully" });
    }
}