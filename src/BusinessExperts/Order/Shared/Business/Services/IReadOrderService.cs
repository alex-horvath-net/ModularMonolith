namespace Experts.Order.Shared.Business.Services;

public interface IReadOrderService {
    Task<Shared.Business.Domain.Order?> GetById(Guid id);
} 