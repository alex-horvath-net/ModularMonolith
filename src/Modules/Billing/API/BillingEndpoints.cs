using Billing.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Billing.API;

public static class BillingEndpoints
{
 public static IEndpointRouteBuilder MapBilling(this IEndpointRouteBuilder app)
 {
 var group = app.MapGroup("/billing");

 group.MapGet("/invoices/{id:guid}", async Task<IResult> (Guid id, BillingDbContext db) =>
 {
 var invoice = await db.Invoices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
 return invoice is null ? Results.NotFound() : Results.Ok(invoice);
 });

 return app;
 }
}
