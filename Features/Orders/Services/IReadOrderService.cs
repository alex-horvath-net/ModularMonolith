using Features.Orders.Domain;

namespace Features.Orders.Services;

public interface IReadOrderService {
    Task<Order?> GetById(Guid id);
}