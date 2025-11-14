namespace Billing.Contracts.DTOs;
public sealed record InvoiceDto(
    Guid Id ,
    Guid OrderId ,
    Guid CustomerId ,
    decimal Total);

