using Experts.Experts.OrderBusinessExpert.Business.DTOs;

namespace Experts.Experts.OrderBusinessExpert.Contracts.Abstraction;

public interface IReadOrderService {
    Task<OrderDto?> GetById(Guid id);
} 