using Billing;
using Common;
using Orders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCommon();
builder.Services.AddOrders(builder.Configuration);
builder.Services.AddBilling(builder.Configuration);
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapOrders();
app.MapBilling();

app.Run();
