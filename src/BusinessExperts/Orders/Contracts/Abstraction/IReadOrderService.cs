using BusinessExperts.Orders.Contracts.DTOs;

namespace BusinessExperts.Orders.Contracts.Abstraction;

public interface IReadOrderService {
    Task<OrderDto?> GetById(Guid id);
} 