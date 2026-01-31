using Experts.OrderExpert.Common.Business.Domain;

namespace Experts.OrderExpert.Common.Business.Services;

public interface IReadOrderService {
    Task<Order?> GetById(Guid id);
}