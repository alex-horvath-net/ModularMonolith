using Billing;
using Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Orders;

var builder = WebApplication.CreateBuilder(args);

// Production-ready minimal JWT validation (Azure AD / OIDC). Configure via appsettings or Key Vault.
// Required keys: Auth:Authority, Auth:Audience (optional Auth:ValidIssuers[])
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.Authority = builder.Configuration["Auth:Authority"] ?? throw new InvalidOperationException("Auth:Authority missing");
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidAudience = builder.Configuration["Auth:Audience"],
            ValidateAudience = true,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Orders.Read", p => p.RequireClaim("scp", "orders.read").RequireAuthenticatedUser())
    .AddPolicy("Orders.Write", p => p.RequireClaim("scp", "orders.write").RequireAuthenticatedUser())
    .AddPolicy("Billing.Read", p => p.RequireClaim("scp", "billing.read").RequireAuthenticatedUser());

builder.Services.AddCommon();
builder.Services.AddOrders(builder.Configuration);
builder.Services.AddBilling(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints then attach authorization at group level (Orders/Billing implement Map* returning route builder only)
var ordersGroup = app.MapOrders();
var billingGroup = app.MapBilling();

// Apply policies to groups (convert to RouteGroupBuilder by re-grouping if needed)
// If MapOrders returns IEndpointRouteBuilder, wrap endpoints directly:
ordersGroup.MapGroup("/orders"); // placeholder if needed

app.Run();
