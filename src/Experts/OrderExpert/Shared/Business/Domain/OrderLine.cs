namespace Experts.OrderExpert.Shared.Business.Domain;

public sealed class OrderLine {
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private OrderLine() { }
    public OrderLine(Guid productId, int quantity, decimal unitPrice) {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId empty.", nameof(productId));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
