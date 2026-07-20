using MassTransit;

namespace OrderService.Sagas;

public class OrderSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;

    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public string CardNumber { get; set; } = string.Empty;
}