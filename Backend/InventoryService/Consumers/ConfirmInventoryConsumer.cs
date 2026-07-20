using MassTransit;
using Microsoft.EntityFrameworkCore;
using InventoryService.Contracts;
using InventoryService.Data;

namespace InventoryService.Consumers;

public class ConfirmInventoryConsumer : IConsumer<ConfirmInventoryCommand>
{
    private readonly InventoryDbContext _context;

    public ConfirmInventoryConsumer(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<ConfirmInventoryCommand> context)
    {
        var inventory = await _context.Inventory
            .FirstOrDefaultAsync(i => i.ProductId == context.Message.ProductId);

        if (inventory == null)
        {
            Console.WriteLine($"Inventory not found for Product {context.Message.ProductId}");
            return;
        }

        inventory.StockQuantity -= context.Message.Quantity;
        inventory.ReservedQuantity -= context.Message.Quantity;
        inventory.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        Console.WriteLine($"CONFIRMED sale of {context.Message.Quantity} units for Order {context.Message.OrderId}");

        await context.Publish(new InventoryConfirmedEvent(context.Message.OrderId));
    }
}