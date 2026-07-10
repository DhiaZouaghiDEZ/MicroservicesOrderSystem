using Microsoft.EntityFrameworkCore;
using MassTransit;
using OrderService.Models.Entities;

namespace OrderService.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //TELL EF CORE TO USE THE 'order' SCHEMA
        modelBuilder.HasDefaultSchema("order");

        // MassTransit Outbox tables will now be created as:
        // order."OutboxState", order."OutboxMessage", etc.
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}