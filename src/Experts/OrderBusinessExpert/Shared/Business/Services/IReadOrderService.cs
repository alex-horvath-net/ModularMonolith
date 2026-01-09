using Experts.OrderBusinessExpert.Shared.Business.Domain;

namespace Experts.OrderBusinessExpert.Shared.Business.Services;

public interface IReadOrderService {
    Task<Order?> GetById(Guid id);
} 