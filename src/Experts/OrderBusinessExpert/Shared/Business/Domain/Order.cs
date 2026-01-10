namespace Business.Experts.OrderBusinessExpert.Shared.Business.Domain;

public sealed class Order {
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }

    private readonly List<OrderLine> _lines = new();
    public IReadOnlyCollection<OrderLine> Lines => _lines;


    public decimal Total => _lines.Sum(l => l.Quantity * l.UnitPrice);

    // Shadow auditing & concurrency handled via configuration.
    public Order() { }

    public Order(Guid customerId) : this(Guid.NewGuid(), customerId) { }

    public Order(Guid id, Guid customerId) {
        if (id == Guid.Empty)
            throw new ArgumentException("Id empty.", nameof(id));
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId empty.", nameof(customerId));

        Id = id;
        CustomerId = customerId;
    }

    public Order(Guid id, Guid customerId, IEnumerable<OrderLine> lines) : this(id, customerId) {
        ArgumentNullException.ThrowIfNull(lines);
        foreach (var line in lines) {
            _lines.Add(line);
        }
    }

    public void AddLine(Guid productId, int quantity, decimal unitPrice) {
       
        _lines.Add(new OrderLine(productId, quantity, unitPrice));
    }

    public Business.Experts.OrderBusinessExpert.Shared.Infrastructure.Data.Models.Order ToInfra() => new() {
        Id = Id,
        CustomerId = CustomerId,
        Lines = Lines.Select(ToInfraOrderLine).ToList()
    };


    private Business.Experts.OrderBusinessExpert.Shared.Infrastructure.Data.Models.OrderLine ToInfraOrderLine(OrderLine domainOrderLine) => new() {
        ProductId = domainOrderLine.ProductId,
        UnitPrice = domainOrderLine.UnitPrice,
        Quantity = domainOrderLine.Quantity
    };
}