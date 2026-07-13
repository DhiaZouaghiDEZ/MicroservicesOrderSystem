using MassTransit;
using PaymentService.Contracts;
using PaymentService.Data;
using PaymentService.Models.Entities;

namespace PaymentService.Consumers;

public class ProcessPaymentConsumer : IConsumer<ProcessPaymentCommand>
{
    private readonly PaymentDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ProcessPaymentConsumer> _logger;

    public ProcessPaymentConsumer(
        PaymentDbContext context,
        IPublishEndpoint publishEndpoint,
        ILogger<ProcessPaymentConsumer> logger)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
    {
        _logger.LogInformation("ProcessPayment: {OrderId} | Amount: {Amount}",
            context.Message.OrderId, context.Message.Amount);

        // Simulate payment logic: success if amount < 1000
        bool paymentSuccess = context.Message.Amount < 1000;

        var paymentRecord = new PaymentRecord
        {
            Id = Guid.NewGuid(),
            OrderId = context.Message.OrderId,
            Amount = context.Message.Amount,
            Status = paymentSuccess ? "Success" : "Failed"
        };

        _context.PaymentRecords.Add(paymentRecord);
        await _context.SaveChangesAsync();

        if (paymentSuccess)
        {
            _logger.LogInformation("Payment successful: {OrderId}", context.Message.OrderId);
            await _publishEndpoint.Publish(new PaymentProcessedEvent(context.Message.OrderId));
        }
        else
        {
            _logger.LogWarning("Payment failed: {OrderId}", context.Message.OrderId);
            await _publishEndpoint.Publish(new PaymentFailedEvent(
                context.Message.OrderId,
                "Payment declined (amount >= 1000)"));
        }
    }
}