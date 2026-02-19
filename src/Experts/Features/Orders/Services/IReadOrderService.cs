using Business.Features.Orders.Domain;

namespace Business.Features.Orders.Services;

public interface IReadOrderService {
    Task<Order?> GetById(Guid id);
}