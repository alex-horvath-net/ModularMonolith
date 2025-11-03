using Microsoft.EntityFrameworkCore;

namespace Billing.Infrastructure;

public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options)
    : DbContext(options) {
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder b) {
        b.HasDefaultSchema("billing");

        b.Entity<Invoice>(cfg => {
            cfg.ToTable("invoices");
            cfg.HasKey(x => x.Id);
            cfg.Property(x => x.CustomerId).IsRequired();
           
        });
    }
}

public sealed class Invoice {
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Total { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public static Invoice Create(Guid orderId,  Guid customerId, decimal total) {
        var invoice = new Invoice { OrderId = orderId, CustomerId = customerId, Total = total };
        return invoice;
    }
}

