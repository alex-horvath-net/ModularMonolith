using Billing.Application;
using Common;
using Orders.API;
using Orders.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IEventBus, InProcessEventBus>();

builder.Services.AddScoped<IEventHandler<OrderPlaced>, InvoiceOnOrderPlaced>();



//builder.Services.AddOrdersInfrastructure(builder.Configuration);
//builder.Services.AddOrdersApplication();

builder.Services.AddOpenApi();



var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapOrders();

app.Run();
