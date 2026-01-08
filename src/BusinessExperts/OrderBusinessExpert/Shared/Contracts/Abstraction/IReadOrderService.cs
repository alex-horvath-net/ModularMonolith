using Experts.OrderBusinessExpert.Shared.Business.DTOs;

namespace Experts.OrderBusinessExpert.Shared.Contracts.Abstraction;

public interface IReadOrderService {
    Task<OrderDto?> GetById(Guid id);
} 