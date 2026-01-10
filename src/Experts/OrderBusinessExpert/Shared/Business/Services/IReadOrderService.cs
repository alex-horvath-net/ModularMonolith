using Business.Experts.OrderBusinessExpert.Shared.Business.Domain;

namespace Business.Experts.OrderBusinessExpert.Shared.Business.Services;

public interface IReadOrderService {
    Task<Order?> GetById(Guid id);
} 