using Billing;
using Common;
using Orders;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.UseKestrel(builder.Configuration, builder.Environment); //// Use consolidated Common Kestrel hardening overload
builder.Host.UseLogger();  // Structured logging configuration

builder.Services.AddCommon(builder.Configuration, builder.Environment);
builder.Services.AddOrders(builder.Configuration);
builder.Services.AddBilling(builder.Configuration);

var app = builder.Build();

app.MapCommon();
app.MapOrders();
app.MapBilling();

app.Run();
