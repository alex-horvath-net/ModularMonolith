using Orders.Core.Domain;

namespace Orders.Core.Services;

public interface IReadOrderService {
    Task<Order?> GetById(Guid id);
}