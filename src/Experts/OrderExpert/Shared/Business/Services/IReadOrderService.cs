namespace Experts.OrderExpert.Shared.Business.Services;

public interface IReadOrderService {
    Task<Domain.Order?> GetById(Guid id);
} 