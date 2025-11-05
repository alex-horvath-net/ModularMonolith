using Billing.Application;
using Common;
using Orders.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Host wires it up without touching internals
builder.Services
    .AddCommon()
    .AddOrders(builder.Configuration);

builder.Services.AddBillingApplication();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapOrders()    ;

app.Run();
