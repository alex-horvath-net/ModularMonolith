using Billing.Application.QueryHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Orders.Contracts.DTOs;
using Asp.Versioning;
using Billing.Contracts.DTOs;

namespace Billing.API;

public static class BillingEndpoints {
    public static IEndpointRouteBuilder MapBilling(this IEndpointRouteBuilder app) {
        var versionSet = app.NewApiVersionSet().HasApiVersion(new ApiVersion(1,0)).ReportApiVersions().Build();
        var group = app.MapGroup("/v{version:apiVersion}/billing")
            .WithApiVersionSet(versionSet)
            .WithTags("Billing")
            .RequireAuthorization("Billing.Read")
            .RequireRateLimiting("fixed"); // Apply read throughput limiter to billing endpoints

        group.MapGet("/invoices/{id:guid}", GetInvoice)
            .Produces<InvoiceDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetInvoice(GetInvoiceQueryHandler handler, Guid id, CancellationToken token) {
        var invoice = await handler.Handle(id, token);
        if (invoice is null) return TypedResults.NotFound();
        var dto = new InvoiceDto(invoice.Id, invoice.OrderId, invoice.CustomerId, invoice.Total);
        return TypedResults.Ok(dto);
    }
}
