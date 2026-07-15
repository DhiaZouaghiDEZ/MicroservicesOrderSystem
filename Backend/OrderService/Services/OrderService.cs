using MassTransit;
using Microsoft.EntityFrameworkCore;
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

    public async Task<Guid> SubmitOrderAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            Amount = request.Amount,
            Status = "Pending"
        };

        _context.Orders.Add(order);

        await _publishEndpoint.Publish(new OrderSubmittedEvent(
            order.Id, order.ProductName, order.Quantity, order.Amount));

        await _context.SaveChangesAsync();

        return order.Id;
    }
    public async Task<List<Order>> GetOrdersAsync()
    {
        return await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
}