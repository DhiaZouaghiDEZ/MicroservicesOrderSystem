using MassTransit;
using Microsoft.EntityFrameworkCore;
using InventoryService.Contracts;
using InventoryService.Data;

namespace InventoryService.Consumers;

public class ReleaseInventoryConsumer : IConsumer<ReleaseInventoryCommand>
{
    private readonly InventoryDbContext _context;

    public ReleaseInventoryConsumer(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<ReleaseInventoryCommand> context)
    {
        var inventory = await _context.Inventory
            .FirstOrDefaultAsync(i => i.ProductId == context.Message.ProductId);

        if (inventory != null)
        {
            inventory.ReservedQuantity -= context.Message.Quantity;
            inventory.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            Console.WriteLine($"RELEASED reservation of {context.Message.Quantity} units for Order {context.Message.OrderId}");
        }

        await context.Publish(new InventoryReleasedEvent(context.Message.OrderId));
    }
}