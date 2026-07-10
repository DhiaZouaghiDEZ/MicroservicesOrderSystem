using MassTransit;
using OrderService.Contracts;
using OrderService.Data;
using OrderService.Models.DTOs;
using OrderService.Models.Entities;

namespace OrderService.Services;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderService(OrderDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> CreateOrderAsync(CreateOrderRequest request)
    {
        // 1. Business Logic & Database Save
        var order = new Order
        {
            Id = Guid.NewGuid(),
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            Status = "Pending"
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // 2. Publish Event to RabbitMQ
        await _publishEndpoint.Publish(new OrderCreatedEvent
        {
            OrderId = order.Id,
            ProductName = order.ProductName,
            Quantity = order.Quantity,
            CreatedAt = order.CreatedAt
        });

        return order.Id;
    }
}