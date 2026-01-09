namespace Experts.OrderBusinessExpert.Shared.Business.Domain;

public sealed class Order {
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }

    private readonly List<OrderLine> _lines = new();
    public IReadOnlyCollection<OrderLine> Lines => _lines;


    public decimal Total => _lines.Sum(l => l.Quantity * l.UnitPrice);

    // Shadow auditing & concurrency handled via configuration.
    private Order() { }

    public Order(Guid customerId) {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId empty.", nameof(customerId));

        Id = Guid.NewGuid();
        CustomerId = customerId;
    }

    public void AddLine(Guid productId, int quantity, decimal unitPrice) {
       
        _lines.Add(new OrderLine(productId, quantity, unitPrice));
    }

    public Infrastructure.Data.Models.Order ToInfra() => new() {
        Id = Id,
        CustomerId = CustomerId,
        Lines = Lines.Select(ToInfraOrderLine).ToList()
    };


    private Infrastructure.Data.Models.OrderLine ToInfraOrderLine(Domain.OrderLine domainOrderLine) => new() {
        ProductId = domainOrderLine.ProductId,
        UnitPrice = domainOrderLine.UnitPrice,
        Quantity = domainOrderLine.Quantity
    };
}