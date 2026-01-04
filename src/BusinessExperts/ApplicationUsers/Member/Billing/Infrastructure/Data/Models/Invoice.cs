namespace Business.ApplicationUsers.Member.Billing.Infrastructure.Data.Models;

public sealed class Invoice {
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Total { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public static Invoice Create(Guid orderId, Guid customerId, decimal total)
    => new() { OrderId = orderId, CustomerId = customerId, Total = total };
}
