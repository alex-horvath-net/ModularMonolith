using Business.OrderBusinessExpert.Contracts.DTOs;

namespace Business.OrderBusinessExpert.Contracts.Abstraction;

public interface IReadOrderService {
    Task<OrderDto?> GetById(Guid id);
} 