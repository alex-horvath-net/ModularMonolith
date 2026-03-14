using Accounts.CreateToken;
using Billing;
using Core;
using Orders;

var builder = WebApplication.CreateBuilder(args);

// Bootstrap configuration: load secrets from Key Vault or user-secrets before any service registration
//builder.Configuration.AddSecretsFromStore(builder.Environment);

//builder.WebHost.UseKestrel(builder.Configuration, builder.Environment); //// Use consolidated Common Kestrel hardening overload
//builder.Host.UseLogger();  // Structured logging configuration

builder.Services.AddCore(builder.Configuration, builder.Environment);
builder.Services.AddOrderBusinessExpert(builder.Configuration);
builder.Services.AddBilling(builder.Configuration);
builder.Services.AddScoped<CreateTokenCommandHandler>();

var app = builder.Build();

app.MapCore();
app.MapDevToken();
app.MapOrders();
app.MapBilling();

app.Run();
