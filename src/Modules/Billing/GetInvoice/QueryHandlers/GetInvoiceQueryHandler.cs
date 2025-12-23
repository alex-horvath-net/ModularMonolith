using Billing.GetInvoice.Contracts.DTOs;
using Billing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Billing.GetInvoice.QueryHandlers;

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
