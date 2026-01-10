using BusinessExperts.BillingBusinessExpert.GetInvoice.Contracts.DTOs;
using BusinessExperts.BillingBusinessExpert.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessExperts.BillingBusinessExpert.GetInvoice.QueryHandlers;

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
