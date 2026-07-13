using MassTransit;

namespace OrderService.Sagas;

public class OrderSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;

    // Data the saga needs to remember while waiting
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
}