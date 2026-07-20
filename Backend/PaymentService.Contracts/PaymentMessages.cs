namespace PaymentService.Contracts;

public record ProcessPaymentCommand(Guid OrderId, decimal Amount, string CardNumber);
public record PaymentProcessedEvent(Guid OrderId);
public record PaymentFailedEvent(Guid OrderId, string Reason);