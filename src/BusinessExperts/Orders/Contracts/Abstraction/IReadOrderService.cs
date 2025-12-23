using Orders.Create.Contracts.DTOs;

namespace Orders.Create.Contracts.Abstraction;

public interface IReadOrderService {
    Task<OrderDto?> GetById(Guid id);
}
 