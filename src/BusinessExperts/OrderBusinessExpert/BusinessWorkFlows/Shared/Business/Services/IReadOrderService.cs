using Experts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business.Domain;

namespace Experts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business.Services;

public interface IReadOrderService {
    Task<Order?> GetById(Guid id);
} 