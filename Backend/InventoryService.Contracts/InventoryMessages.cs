namespace InventoryService.Contracts;

// Commands the Saga sends to Inventory
public record ReserveInventoryCommand(Guid OrderId, string ProductName, int Quantity);
public record ReleaseInventoryCommand(Guid OrderId, string ProductName, int Quantity);

// Events Inventory sends back to the Saga
public record InventoryReservedEvent(Guid OrderId);
public record InventoryReservationFailedEvent(Guid OrderId, string Reason);