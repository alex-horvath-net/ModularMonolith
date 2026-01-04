using Business.MemberApplicationUser.OrderBusinessExpert.Contracts.DTOs;

namespace Business.MemberApplicationUser.OrderBusinessExpert.Contracts.Abstraction;

public interface IReadOrderService {
    Task<OrderDto?> GetById(Guid id);
} 