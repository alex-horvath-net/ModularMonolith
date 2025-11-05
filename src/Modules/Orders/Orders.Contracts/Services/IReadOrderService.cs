using Orders.Contracts.DTOs;

namespace Orders.Contracts.Services;

// Don’t allow random DbContext access across modules.
// Oprion 1:
// Billing calls only Orders.Contracts, because only that exposes public interface and  Orders.Application implement it with internal.
// Oprion 2:
// Orders publishes events; Billing stores what it needs. 
public interface IReadOrderService {
    Task<OrderDto?> GetById(Guid id);
}
  