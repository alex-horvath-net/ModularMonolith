namespace Orders.Infrastructure.Data.Models;

// Owned type; simple value object
public sealed class OrderLine {
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private OrderLine() { } // For EF

    public OrderLine(Guid productId, int quantity, decimal unitPrice) {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
