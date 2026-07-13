User → OrderService API → [OrderSubmittedEvent]
                                    ↓
                          Order Saga (Orchestrator)
                                    ↓
                    [ReserveInventoryCommand]
                                    ↓
                          InventoryService
                                    ↓
                    [InventoryReservedEvent]
                                    ↓
                          Order Saga
                                    ↓
                      [ProcessPaymentCommand]
                                    ↓
                          PaymentService
                                    ↓
              ┌─────────────────────┴─────────────────────┐
              ↓                                           ↓
    [PaymentProcessedEvent]                    [PaymentFailedEvent]
              ↓                                           ↓
        Order Saga                                   Order Saga
              ↓                                           ↓
    [UpdateOrderStatusCommand]              [ReleaseInventoryCommand]
              ↓                                           ↓
        OrderService DB                          InventoryService
    (Status: Completed)                    (Undo stock reservation)
                                                    ↓
                                          [UpdateOrderStatusCommand]
                                                    ↓
                                              OrderService DB
                                          (Status: Failed)