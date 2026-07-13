using Microsoft.EntityFrameworkCore;
using MassTransit;
using OrderService.Models.Entities;
using OrderService.Sagas;

namespace OrderService.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("order");
    
        modelBuilder.Entity<OrderSagaState>(builder =>
        {
            builder.HasKey(x => x.CorrelationId);
            builder.Property(x => x.CurrentState).HasMaxLength(64);
            builder.Property(x => x.ProductName).HasMaxLength(256);
        });

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}