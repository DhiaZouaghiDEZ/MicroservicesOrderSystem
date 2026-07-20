using OrderService.Models.DTOs;

namespace OrderService.Services;

public interface IOrderService
{
    Task<Guid> SubmitOrderAsync(CreateOrderRequest request);
    Task<IEnumerable<Models.Entities.Order>> GetAllOrdersAsync();
}