using OrderService.Models.DTOs;
using OrderService.Models.Entities;

namespace OrderService.Services;

public interface IOrderService
{
    Task<Guid> SubmitOrderAsync(CreateOrderRequest request);
    Task<List<Order>> GetOrdersAsync();
}