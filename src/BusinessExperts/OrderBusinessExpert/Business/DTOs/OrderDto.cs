namespace Experts.OrderBusinessExpert.Business.DTOs;

public sealed record OrderDto(Guid Id, Guid CustomerId, decimal Total);
