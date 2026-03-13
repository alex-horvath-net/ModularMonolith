namespace Orders.Core.Infrastructure.Data.Models;

public sealed class Order {
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public List<OrderLine> Lines { get; set; } = [];

    public Domain.Order ToDomain() {
        var orderDomain = new Domain.Order(Id, CustomerId);

        foreach (var line in Lines)
            orderDomain.AddLine(line.ProductId, line.Quantity, line.UnitPrice);
        return orderDomain;
    }
}