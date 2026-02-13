using Business.Modules.OrderExpert.Common.Business.Domain;

namespace Business.Modules.OrderExpert.Common.Business.Services;

public interface IReadOrderService {
    Task<Order?> GetById(Guid id);
}