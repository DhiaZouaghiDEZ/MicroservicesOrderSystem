using MassTransit;
using Microsoft.EntityFrameworkCore;
using InventoryService.Contracts;
using InventoryService.Data;

namespace InventoryService.Consumers;

public class ReleaseInventoryConsumer : IConsumer<ReleaseInventoryCommand>
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<ReleaseInventoryConsumer> _logger;

    public ReleaseInventoryConsumer(InventoryDbContext context, ILogger<ReleaseInventoryConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReleaseInventoryCommand> context)
    {
        _logger.LogInformation("COMPENSATING: ReleaseInventory | {OrderId} | {ProductName} x {Quantity}",
            context.Message.OrderId, context.Message.ProductName, context.Message.Quantity);

        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductName == context.Message.ProductName);

        if (item != null)
        {
            item.StockQuantity += context.Message.Quantity;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Inventory released. Stock restored to {Stock}", item.StockQuantity);
        }
    }
}