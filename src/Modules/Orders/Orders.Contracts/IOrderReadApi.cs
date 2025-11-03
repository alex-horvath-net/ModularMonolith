namespace Orders.Contracts;

public interface IOrderReadApi {
    Task<OrderDto?> GetById(Guid id);
}


public sealed record OrderDto(Guid Id, Guid CustomerId, decimal Total);
