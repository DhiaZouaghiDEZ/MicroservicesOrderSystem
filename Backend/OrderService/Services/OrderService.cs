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
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public OrderService(
        OrderDbContext context,
        IPublishEndpoint publishEndpoint,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<Guid> SubmitOrderAsync(CreateOrderRequest request)
    {
        var client = _httpClientFactory.CreateClient();
        var inventoryServiceUrl = _configuration["InventoryService:Url"] ?? "https://localhost:44392";

        var response = await client.GetAsync($"{inventoryServiceUrl}/api/inventory/products/{request.ProductId}/basic");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Product {request.ProductId} not found");
        }

        var productData = await response.Content.ReadFromJsonAsync<ProductDto>();
        if (productData == null)
        {
            throw new Exception("Invalid product data");
        }

        var amount = productData.Price * request.Quantity;

        var order = new Order
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            ProductName = productData.ProductName,
            Quantity = request.Quantity,
            Amount = amount,
            Status = "Pending"
        };

        _context.Orders.Add(order);

        await _publishEndpoint.Publish(new OrderSubmittedEvent(
            order.Id,
            request.ProductId,
            request.Quantity,
            amount,
            request.CardNumber));

        await _context.SaveChangesAsync();

        return order.Id;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
}

// DTO for deserializing product data from InventoryService
public class ProductDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}