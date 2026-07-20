using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.Contracts;
using PaymentService.Data;
using PaymentService.Models.Entities;

namespace PaymentService.Consumers;

public class ProcessPaymentConsumer : IConsumer<ProcessPaymentCommand>
{
    private readonly PaymentDbContext _context;

    public ProcessPaymentConsumer(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
    {
        Console.WriteLine($"Processing payment for Order {context.Message.OrderId}, Amount: {context.Message.Amount}");

        //Validate credit card exists
        var card = await _context.CreditCards
            .FirstOrDefaultAsync(c => c.CardNumber == context.Message.CardNumber);

        if (card == null)
        {
            Console.WriteLine($"Invalid card number");
            await context.Publish(new PaymentFailedEvent(context.Message.OrderId, "Invalid credit card"));
            return;
        }

        //Check expiry (simple check)
        if (!DateTime.TryParseExact(card.ExpiryDate, "MM/yy", null, System.Globalization.DateTimeStyles.None, out var expiry))
        {
            await context.Publish(new PaymentFailedEvent(context.Message.OrderId, "Invalid expiry date format"));
            return;
        }

        if (expiry < DateTime.Now)
        {
            await context.Publish(new PaymentFailedEvent(context.Message.OrderId, "Card expired"));
            return;
        }

        //Check balance
        if (card.Balance < context.Message.Amount)
        {
            await context.Publish(new PaymentFailedEvent(
                context.Message.OrderId,
                $"Insufficient funds. Balance: {card.Balance}, Required: {context.Message.Amount}"));
            return;
        }

        //Process payment
        card.Balance -= context.Message.Amount;

        var paymentRecord = new PaymentRecord
        {
            Id = Guid.NewGuid(),
            OrderId = context.Message.OrderId,
            Amount = context.Message.Amount,
            Status = "Completed",
            ProcessedAt = DateTime.UtcNow
        };

        _context.PaymentRecords.Add(paymentRecord);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Payment successful for Order {context.Message.OrderId}");

        await context.Publish(new PaymentProcessedEvent(context.Message.OrderId));
    }
}