namespace Business.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.Infrastructure.Data.Models;

public sealed class OrderLine {
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
}
