using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryService.Data;
using InventoryService.Models.Entities;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryDbContext _context;

    public InventoryController(InventoryDbContext context)
    {
        _context = context;
    }

    // Endpoint to add stock manually
    [HttpPost("add")]
    public async Task<IActionResult> AddStock([FromBody] AddStockRequest request)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductName == request.ProductName);

        if (item == null)
        {
            item = new InventoryItem { ProductName = request.ProductName, StockQuantity = request.Quantity };
            _context.InventoryItems.Add(item);
        }
        else
        {
            item.StockQuantity += request.Quantity;
        }

        await _context.SaveChangesAsync();
        return Ok(new { Message = "Stock updated", item });
    }

    // Endpoint to view all stock
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _context.InventoryItems.ToListAsync();
        return Ok(items);
    }
}

public record AddStockRequest(string ProductName, int Quantity);