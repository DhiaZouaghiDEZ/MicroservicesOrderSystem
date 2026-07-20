namespace OrderService.Contracts;

public record OrderSubmittedEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    decimal Amount,
    string CardNumber);

public record UpdateOrderStatusCommand(Guid OrderId, string NewStatus);