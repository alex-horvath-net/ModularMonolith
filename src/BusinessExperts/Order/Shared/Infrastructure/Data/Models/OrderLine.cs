namespace Experts.Order.Shared.Infrastructure.Data.Models;

public sealed class OrderLine {
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
