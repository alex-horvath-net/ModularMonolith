using BusinessExperts.OrderBusinessExpert.Shared.Business.Domain;

namespace BusinessExperts.OrderBusinessExpert.Shared.Business.Services;

public interface IReadOrderService {
    Task<Order?> GetById(Guid id);
} 