using MassTransit;
using OrderService.Contracts;
using InventoryService.Contracts;
using PaymentService.Contracts;

namespace OrderService.Sagas;

public class OrderSagaStateMachine : MassTransitStateMachine<OrderSagaState>
{
    public Event<OrderSubmittedEvent> OrderSubmitted { get; private set; } = null!;
    public Event<InventoryReservedEvent> InventoryReserved { get; private set; } = null!;
    public Event<InventoryReservationFailedEvent> InventoryReservationFailed { get; private set; } = null!;
    public Event<InventoryConfirmedEvent> InventoryConfirmed { get; private set; } = null!;
    public Event<InventoryReleasedEvent> InventoryReleased { get; private set; } = null!;
    public Event<PaymentProcessedEvent> PaymentProcessed { get; private set; } = null!;
    public Event<PaymentFailedEvent> PaymentFailed { get; private set; } = null!;

    public OrderSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmitted, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => InventoryReserved, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => InventoryReservationFailed, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => InventoryConfirmed, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => InventoryReleased, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => PaymentProcessed, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => PaymentFailed, x => x.CorrelateById(ctx => ctx.Message.OrderId));

        Initially(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.ProductId = context.Message.ProductId;
                    context.Saga.Quantity = context.Message.Quantity;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.CardNumber = context.Message.CardNumber;
                })
                .Publish(context => new ReserveInventoryCommand(
                    context.Saga.CorrelationId,
                    context.Saga.ProductId,
                    context.Saga.Quantity))
                .TransitionTo(PendingInventory)
        );

        During(PendingInventory,
            When(InventoryReserved)
                .Publish(context => new ProcessPaymentCommand(
                    context.Saga.CorrelationId,
                    context.Saga.Amount,
                    context.Saga.CardNumber))
                .TransitionTo(PendingPayment),
            When(InventoryReservationFailed)
                .Publish(context => new UpdateOrderStatusCommand(
                    context.Saga.CorrelationId,
                    $"Failed: {context.Message.Reason}"))
                .Finalize()
        );

        During(PendingPayment,
            When(PaymentProcessed)
                .Publish(context => new ConfirmInventoryCommand(
                    context.Saga.CorrelationId,
                    context.Saga.ProductId,
                    context.Saga.Quantity))
                .TransitionTo(ConfirmingInventory),
            When(PaymentFailed)
                .Publish(context => new ReleaseInventoryCommand(
                    context.Saga.CorrelationId,
                    context.Saga.ProductId,
                    context.Saga.Quantity))
                .TransitionTo(ReleasingInventory)
        );

        During(ConfirmingInventory,
            When(InventoryConfirmed)
                .Publish(context => new UpdateOrderStatusCommand(
                    context.Saga.CorrelationId, "Completed"))
                .Finalize()
        );

        During(ReleasingInventory,
            When(InventoryReleased)
                .Publish(context => new UpdateOrderStatusCommand(
                    context.Saga.CorrelationId, "Failed: Payment declined"))
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    public State PendingInventory { get; private set; } = null!;
    public State PendingPayment { get; private set; } = null!;
    public State ConfirmingInventory { get; private set; } = null!;
    public State ReleasingInventory { get; private set; } = null!;
}