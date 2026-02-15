using Business.Experts.OrderExpert.Common.Business.Domain;

namespace Business.Experts.OrderExpert.Common.Business.Services;

public interface IReadOrderService {
    Task<Order?> GetById(Guid id);
}