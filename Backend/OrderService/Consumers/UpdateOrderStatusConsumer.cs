using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Contracts;
using OrderService.Data;

namespace OrderService.Consumers;

public class UpdateOrderStatusConsumer : IConsumer<UpdateOrderStatusCommand>
{
    private readonly OrderDbContext _context;

    public UpdateOrderStatusConsumer(OrderDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<UpdateOrderStatusCommand> context)
    {
        var order = await _context.Orders.FindAsync(context.Message.OrderId);
        if (order != null)
        {
            order.Status = context.Message.NewStatus;
            await _context.SaveChangesAsync();
        }
    }
}