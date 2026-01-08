using Experts.OrderBusinessExpert.Business.DTOs;

namespace Experts.OrderBusinessExpert.Contracts.Abstraction;

public interface IReadOrderService {
    Task<OrderDto?> GetById(Guid id);
} 