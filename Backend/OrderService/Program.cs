using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    // Configure the Entity Framework Outbox
    x.AddEntityFrameworkOutbox<OrderDbContext>(o =>
    {
        o.UsePostgres(); // Tell it we are using PostgreSQL
        o.UseBusOutbox(); // Ensure messages sent from consumers also use the outbox
    });
       
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["RabbitMq:Host"]!));

        // Tell RabbitMQ to use the outbox for publishing
        cfg.UseMessageRetry(r => r.Interval(3, 5000)); // Retry 3 times if RabbitMQ is briefly unavailable
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