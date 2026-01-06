namespace BusinessExperts.Order.Contracts.DTOs;

public sealed record OrderDto(Guid Id, Guid CustomerId, decimal Total);
