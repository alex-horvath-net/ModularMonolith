using Billing.Infrastructure.Data;
using Billing.Slices.GetInvoice.Contracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Billing.Slices.GetInvoice.QueryHandlers;

public sealed class GetInvoiceQueryHandler(BillingDbContext db) {
    public async Task<InvoiceDto?> Handle(Guid id, CancellationToken token) {
        var invoice = await db.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, token);

        if (invoice is null)
            return null;

        return new InvoiceDto(invoice.Id, invoice.OrderId, invoice.CustomerId, invoice.Total);
    }
}
