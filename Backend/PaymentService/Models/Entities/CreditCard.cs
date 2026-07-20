namespace PaymentService.Models.Entities;

public class CreditCard
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; } = string.Empty; 
    public string CardHolderName { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
    public string CVV { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}