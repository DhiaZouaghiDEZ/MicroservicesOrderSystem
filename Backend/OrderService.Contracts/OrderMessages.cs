namespace OrderService.Contracts;

// Events that start the workflow
public record OrderSubmittedEvent(Guid OrderId, string ProductName, int Quantity, decimal Amount);

// Internal command to update local DB
public record UpdateOrderStatusCommand(Guid OrderId, string NewStatus);