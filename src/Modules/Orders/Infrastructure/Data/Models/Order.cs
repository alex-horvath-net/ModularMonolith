namespace Orders.Infrastructure.Data.Models;

// Changed from positional record (EF couldn't bind navigation in ctor) to standard class with EF-friendly constructor
public sealed class Order {
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }

    private readonly List<OrderLine> _lines = new();
    public IReadOnlyList<OrderLine> Lines => _lines;

    // Parameterless constructor required by EF Core
    private Order() { }

    private Order(Guid id, Guid customerId) {
        Id = id;
        CustomerId = customerId;
    }

    public static Order Create(Guid customerId, IEnumerable<(Guid productId, int quantity, decimal unitPrice)> lines) {
        var order = new Order(Guid.NewGuid(), customerId);
        foreach (var (productId, quantity, unitPrice) in lines) {
            order._lines.Add(new OrderLine(productId, quantity, unitPrice));
        }
        return order;
    }
}
