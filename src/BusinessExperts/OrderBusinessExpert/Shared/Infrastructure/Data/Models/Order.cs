namespace Experts.OrderBusinessExpert.Shared.Infrastructure.Data.Models;

public sealed class Order {
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }

    public IReadOnlyCollection<OrderLine> Lines { get; set; } = [];
}