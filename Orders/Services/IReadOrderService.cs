using Orders.Domain;

namespace Orders.Services;

public interface IReadOrderService {
    Task<Order?> GetById(Guid id);
}