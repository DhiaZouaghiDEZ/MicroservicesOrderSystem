namespace OrderService.Models.DTOs
{
    public class CreateOrderRequest
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
