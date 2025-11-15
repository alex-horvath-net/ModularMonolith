using Billing;
using Common;
using Orders;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCommon(builder.Configuration);
builder.Services.AddOrders(builder.Configuration);
builder.Services.AddBilling(builder.Configuration);


var app = builder.Build();

app.MapCommon();
app.MapOrders();
app.MapBilling();


app.Run();
