using BusinessExperts.Order.Contracts.DTOs;

namespace BusinessExperts.Order.Contracts.Abstraction;

public interface IReadOrderService {
    Task<OrderDto?> GetById(Guid id);
} 