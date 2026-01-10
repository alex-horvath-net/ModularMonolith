using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business.Domain;

namespace BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business.Services;

public interface IReadOrderService {
    Task<Order?> GetById(Guid id);
} 