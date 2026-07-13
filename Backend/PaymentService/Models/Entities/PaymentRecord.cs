namespace PaymentService.Models.Entities;

public class PaymentRecord
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Success, Failed
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}