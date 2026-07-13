using MassTransit;
using Microsoft.EntityFrameworkCore;
using InventoryService.Contracts;
using InventoryService.Data;

namespace InventoryService.Consumers;

public class ReserveInventoryConsumer : IConsumer<ReserveInventoryCommand>
{
    private readonly InventoryDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ReserveInventoryConsumer> _logger;

    public ReserveInventoryConsumer(
        InventoryDbContext context,
        IPublishEndpoint publishEndpoint,
        ILogger<ReserveInventoryConsumer> logger)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReserveInventoryCommand> context)
    {
        _logger.LogInformation("ReserveInventory: {OrderId} | {ProductName} x {Quantity}",
            context.Message.OrderId, context.Message.ProductName, context.Message.Quantity);

        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductName == context.Message.ProductName);

        if (item == null || item.StockQuantity < context.Message.Quantity)
        {
            _logger.LogWarning("Failed: Insufficient stock for {ProductName}", context.Message.ProductName);

            await _publishEndpoint.Publish(new InventoryReservationFailedEvent(
                context.Message.OrderId,
                item == null ? "Product not found" : "Insufficient stock"));
            return;
        }

        item.StockQuantity -= context.Message.Quantity;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Stock reserved for {ProductName}", context.Message.ProductName);

        await _publishEndpoint.Publish(new InventoryReservedEvent(context.Message.OrderId));
    }
}