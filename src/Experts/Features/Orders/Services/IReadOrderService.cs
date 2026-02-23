using Core.Features.Orders.Domain;

namespace Core.Features.Orders.Services;

public interface IReadOrderService {
    Task<Order?> GetById(Guid id);
}