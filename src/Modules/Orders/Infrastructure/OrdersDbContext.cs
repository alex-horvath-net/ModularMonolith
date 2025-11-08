using Microsoft.EntityFrameworkCore;

namespace Orders.Infrastructure;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options) {
    public DbSet<Order> Orders => Set<Order>();
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Order>(e => {
            e.HasKey(o => o.Id);
            e.OwnsMany(o => o.Lines, b => {
                b.WithOwner().HasForeignKey("OrderId");
                b.Property<Guid>("Id");
                b.HasKey("Id");
            });
        });
    }
}

public sealed record Order(Guid Id, Guid CustomerId, IReadOnlyList<OrderLine> Lines) {
    public static Order Create(Guid customerId, IEnumerable<(Guid productId, int quantity, decimal unitPrice)> lines)
    => new(Guid.NewGuid(), customerId, lines.Select(l => new OrderLine(l.productId, l.quantity, l.unitPrice)).ToList());
}

public sealed record OrderLine(Guid ProductId, int Quantity, decimal UnitPrice);
