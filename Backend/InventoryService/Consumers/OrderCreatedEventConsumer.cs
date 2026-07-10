using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Contracts;
using InventoryService.Data;

namespace InventoryService.Consumers;

public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<OrderCreatedEventConsumer> _logger;

    public OrderCreatedEventConsumer(InventoryDbContext context, ILogger<OrderCreatedEventConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        _logger.LogInformation("Received OrderCreatedEvent for OrderId: {OrderId}", context.Message.OrderId);

        // 1. Find the product in our isolated inventory database
        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductName == context.Message.ProductName);

        if (item == null)
        {
            _logger.LogWarning("Product {ProductName} not found in inventory!", context.Message.ProductName);
            // In a real app, you might publish an OrderFailedEvent here.
            // For now, we just acknowledge and move on.
            return;
        }

        // 2. Deduct the stock
        if (item.StockQuantity >= context.Message.Quantity)
        {
            item.StockQuantity -= context.Message.Quantity;
            _logger.LogInformation("Stock updated for {ProductName}. New stock: {Stock}", item.ProductName, item.StockQuantity);
        }
        else
        {
            _logger.LogWarning("Insufficient stock for {ProductName}. Available: {Available}, Requested: {Requested}",
                item.ProductName, item.StockQuantity, context.Message.Quantity);
            item.StockQuantity = 0; // Set to 0 or handle backorder logic
        }

        // 3. Save to the database
        // Because we configured the EF Core Outbox in Program.cs, 
        // this save and the RabbitMQ acknowledgment are wrapped in a safe transaction.
        await _context.SaveChangesAsync();
    }
}