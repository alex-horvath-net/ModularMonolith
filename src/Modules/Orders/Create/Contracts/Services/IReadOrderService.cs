using Orders.Create.Contracts.DTOs;

namespace Orders.Create.Contracts.Services;

public interface IReadOrderService {
    Task<OrderDto?> GetById(Guid id);
}
