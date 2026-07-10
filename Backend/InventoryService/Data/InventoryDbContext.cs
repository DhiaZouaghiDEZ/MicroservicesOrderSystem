using Microsoft.EntityFrameworkCore;
using MassTransit;
using InventoryService.Models.Entities;

namespace InventoryService.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<InventoryItem> InventoryItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //TELL EF CORE TO USE THE 'inventory' SCHEMA
        modelBuilder.HasDefaultSchema("inventory");

        // MassTransit Outbox tables will now be created as:
        // inventory."OutboxState", inventory."OutboxMessage", etc.
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}