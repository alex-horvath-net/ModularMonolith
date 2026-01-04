using Business.MemberApplicationUser.BillingBusinessExpert;
using Business.MemberApplicationUser.IdentityBusinessExpert.CreateToken;
using Business.MemberApplicationUser.OrderBusinessExpert;
using Common;

var builder = WebApplication.CreateBuilder(args);

// Bootstrap configuration: load secrets from Key Vault or user-secrets before any service registration
//builder.Configuration.AddSecretsFromStore(builder.Environment);

//builder.WebHost.UseKestrel(builder.Configuration, builder.Environment); //// Use consolidated Common Kestrel hardening overload
//builder.Host.UseLogger();  // Structured logging configuration

builder.Services.AddCommon(builder.Configuration, builder.Environment);
builder.Services.AddOrders(builder.Configuration);
builder.Services.AddBilling(builder.Configuration);
builder.Services.AddScoped<CreateTokenCommandHandler>();

var app = builder.Build();

app.MapCommon();
app.MapDevToken();
app.MapOrders();
app.MapBilling();

app.Run();

public partial class Program { }
