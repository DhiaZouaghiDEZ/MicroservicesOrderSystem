using MassTransit;
using Microsoft.EntityFrameworkCore;
using InventoryService.Contracts;
using InventoryService.Data;

namespace InventoryService.Consumers;

public class ReserveInventoryConsumer : IConsumer<ReserveInventoryCommand>
{
    private readonly InventoryDbContext _context;

    public ReserveInventoryConsumer(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<ReserveInventoryCommand> context)
    {
        var inventory = await _context.Inventory
            .FirstOrDefaultAsync(i => i.ProductId == context.Message.ProductId);

        if (inventory == null)
        {
            await context.Publish(new InventoryReservationFailedEvent(
                context.Message.OrderId, "Product not found in inventory"));
            return;
        }

        if (inventory.AvailableQuantity < context.Message.Quantity)
        {
            await context.Publish(new InventoryReservationFailedEvent(
                context.Message.OrderId,
                $"Insufficient stock. Available: {inventory.AvailableQuantity}, Requested: {context.Message.Quantity}"));
            return;
        }

        inventory.ReservedQuantity += context.Message.Quantity;
        inventory.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        Console.WriteLine($"🔒 RESERVED {context.Message.Quantity} units of Product {context.Message.ProductId} for Order {context.Message.OrderId}");

        await context.Publish(new InventoryReservedEvent(context.Message.OrderId));
    }
}