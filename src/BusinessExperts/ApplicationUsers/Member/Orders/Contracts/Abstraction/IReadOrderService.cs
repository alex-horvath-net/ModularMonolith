using Business.ApplicationUsers.Member.Orders.Contracts.DTOs;

namespace Business.ApplicationUsers.Member.Orders.Contracts.Abstraction;

public interface IReadOrderService {
    Task<OrderDto?> GetById(Guid id);
} 