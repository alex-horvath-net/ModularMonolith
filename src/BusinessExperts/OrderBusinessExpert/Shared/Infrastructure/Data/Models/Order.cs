namespace Experts.OrderBusinessExpert.Shared.Infrastructure.Data.Models;

public sealed class Order {
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }

    public IReadOnlyCollection<OrderLine> Lines { get; private set; } = [];
}