using Orders.Contracts.DTOs;

namespace Orders.Contracts.Services;

public interface IReadOrderService {
    Task<OrderDto?> GetById(Guid id);
}
