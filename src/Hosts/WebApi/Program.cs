using Common;
using Orders; // unified Orders module
using Billing; // unified Billing module

var builder = WebApplication.CreateBuilder(args);

builder.Services
 .AddCommon()
 .AddOrders(builder.Configuration)
 .AddBilling(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
 app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapOrders();
app.MapBilling();

app.Run();
