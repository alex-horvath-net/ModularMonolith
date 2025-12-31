namespace BusinessExperts.Orders.Featrures.Create.Infrastructure.Data.Models;

public sealed class Order {
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }

    private readonly List<OrderLine> _lines = new();
    public IReadOnlyCollection<OrderLine> Lines => _lines;

    // Shadow auditing & concurrency handled via configuration.
    private Order() { }

    private Order(Guid id, Guid customerId) {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId empty.", nameof(customerId));
        Id = id;
        CustomerId = customerId;
    }

    public static Order Create(Guid customerId, IEnumerable<(Guid productId, int quantity, decimal unitPrice)> lines) {
        var order = new Order(Guid.NewGuid(), customerId);
        foreach (var (productId, quantity, unitPrice) in lines)
            order.AddLine(productId, quantity, unitPrice);
        return order;
    }

    public void AddLine(Guid productId, int quantity, decimal unitPrice) {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId empty.", nameof(productId));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));
        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice));
        _lines.Add(new OrderLine(productId, quantity, unitPrice));
    }
}