using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Sagas;
using OrderService.Services;
using Scalar.AspNetCore;
using OrderService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    // 1. Register the local DB updater consumer
    x.AddConsumer<UpdateOrderStatusConsumer>();

    x.AddSagaStateMachine<OrderSagaStateMachine, OrderSagaState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Optimistic;
            r.UsePostgres();
            r.ExistingDbContext<OrderDbContext>();
        });

    // 3. Configure the Outbox
    x.AddEntityFrameworkOutbox<OrderDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
    });

    // 4. RabbitMQ Configuration
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["RabbitMq:Host"]!));
        cfg.UseMessageRetry(r => r.Interval(3, 5000));

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();
var app = builder.Build();

// Configure Native OpenAPI & Scalar UI
if (app.Environment.IsDevelopment())
{
    // Generates the OpenAPI JSON document at /openapi/v1.json
    app.MapOpenApi();

    // Mounts the beautiful Scalar UI at /scalar/v1
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/scalar/v1"));

app.Run();