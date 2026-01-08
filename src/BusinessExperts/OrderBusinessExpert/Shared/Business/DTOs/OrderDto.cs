namespace Experts.OrderBusinessExpert.Shared.Business.DTOs;

public sealed record OrderDto(Guid Id, Guid CustomerId, decimal Total);
