namespace InventoryService.Contracts;

// Commands (from Saga to InventoryService)
public record ReserveInventoryCommand(Guid OrderId, Guid ProductId, int Quantity);
public record ConfirmInventoryCommand(Guid OrderId, Guid ProductId, int Quantity);
public record ReleaseInventoryCommand(Guid OrderId, Guid ProductId, int Quantity);

// Events (from InventoryService to Saga)
public record InventoryReservedEvent(Guid OrderId);
public record InventoryConfirmedEvent(Guid OrderId);
public record InventoryReleasedEvent(Guid OrderId);
public record InventoryReservationFailedEvent(Guid OrderId, string Reason);