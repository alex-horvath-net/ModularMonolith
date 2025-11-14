using Billing.Application.QueryHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Billing.API;

public static class BillingEndpoints {
    public static IEndpointRouteBuilder MapBilling(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/billing");

        group.MapGet("/invoices/{id:guid}", GetInvoice);

        return app;
    }

    private static async Task<IResult> GetInvoice(GetInvoiceQueryHandler handler, Guid id, CancellationToken token) {
        var invoice = await handler.Handle(id, token);
        return invoice is null ? TypedResults.NotFound() : TypedResults.Ok(invoice);
    }
}
