using MassTransit;
using OrderService.Contracts;
using InventoryService.Contracts;
using PaymentService.Contracts;

namespace OrderService.Sagas;

public class OrderSagaStateMachine : MassTransitStateMachine<OrderSagaState>
{
    // Define the states
    public State PendingInventory { get; private set; } = null!;
    public State PendingPayment { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State Failed { get; private set; } = null!;

    // Define the events that trigger transitions
    public Event<OrderSubmittedEvent> OrderSubmitted { get; private set; } = null!;
    public Event<InventoryReservedEvent> InventoryReserved { get; private set; } = null!;
    public Event<InventoryReservationFailedEvent> InventoryFailed { get; private set; } = null!;
    public Event<PaymentProcessedEvent> PaymentProcessed { get; private set; } = null!;
    public Event<PaymentFailedEvent> PaymentFailed { get; private set; } = null!;

    public OrderSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        // Correlate all events by OrderId
        Event(() => OrderSubmitted, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => InventoryReserved, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => InventoryFailed, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => PaymentProcessed, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => PaymentFailed, x => x.CorrelateById(ctx => ctx.Message.OrderId));

        // INITIAL STATE: Order submitted -> Reserve inventory
        Initially(
            When(OrderSubmitted)
                .Then(ctx =>
                {
                    ctx.Saga.ProductName = ctx.Message.ProductName;
                    ctx.Saga.Quantity = ctx.Message.Quantity;
                    ctx.Saga.Amount = ctx.Message.Amount;
                })
                .TransitionTo(PendingInventory)
                .Publish(ctx => new ReserveInventoryCommand(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.ProductName,
                    ctx.Saga.Quantity))
        );

        // PENDING INVENTORY: Success -> Process payment
        During(PendingInventory,
            When(InventoryReserved)
                .TransitionTo(PendingPayment)
                .Publish(ctx => new ProcessPaymentCommand(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.Amount,
                    "4111111111111111")) // Fake card number
        );

        // PENDING INVENTORY: Failure -> Fail order
        During(PendingInventory,
            When(InventoryFailed)
                .TransitionTo(Failed)
                .Publish(ctx => new UpdateOrderStatusCommand(ctx.Saga.CorrelationId, "Failed"))
                .Finalize()
        );

        // PENDING PAYMENT: Success -> Complete order
        During(PendingPayment,
            When(PaymentProcessed)
                .TransitionTo(Completed)
                .Publish(ctx => new UpdateOrderStatusCommand(ctx.Saga.CorrelationId, "Completed"))
                .Finalize()
        );

        // PENDING PAYMENT: Failure -> Release inventory (COMPENSATING!)
        During(PendingPayment,
            When(PaymentFailed)
                .TransitionTo(Failed)
                .Publish(ctx => new ReleaseInventoryCommand(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.ProductName,
                    ctx.Saga.Quantity))
                .Publish(ctx => new UpdateOrderStatusCommand(ctx.Saga.CorrelationId, "Failed"))
                .Finalize()
        );
    }
}