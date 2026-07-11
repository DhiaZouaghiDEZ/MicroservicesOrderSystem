using MassTransit;
using Microsoft.EntityFrameworkCore;
using InventoryService.Consumers;
using InventoryService.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Explicit Configuration Loading
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 1. Database
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. MassTransit & Consumer
builder.Services.AddMassTransit(x =>
{
    // 1. Register the consumer
    x.AddConsumer<OrderCreatedEventConsumer>();

    // 2. Configure the Outbox
    x.AddEntityFrameworkOutbox<InventoryDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["RabbitMq:Host"]!));
        cfg.UseMessageRetry(r => r.Interval(3, 5000));

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => Results.Redirect("/scalar/v1"));
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();