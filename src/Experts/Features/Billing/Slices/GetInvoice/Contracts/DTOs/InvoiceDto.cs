namespace Business.Features.Billing.Slices.GetInvoice.Contracts.DTOs;

public sealed record InvoiceDto(
    Guid Id,
    Guid OrderId,
    Guid CustomerId,
    decimal Total);

