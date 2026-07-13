namespace PaymentService.Contracts;

// Command the Saga sends to Payment
public record ProcessPaymentCommand(Guid OrderId, decimal Amount, string CardNumber);

// Events Payment sends back to the Saga
public record PaymentProcessedEvent(Guid OrderId);
public record PaymentFailedEvent(Guid OrderId, string Reason);