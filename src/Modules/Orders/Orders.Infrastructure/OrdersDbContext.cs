using Microsoft.EntityFrameworkCore;

namespace Orders.Infrastructure;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options)
    : DbContext(options) {
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasDefaultSchema("orders");

        modelBuilder.Entity<Order>(entity => {
            entity.ToTable("orders");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CustomerId).IsRequired();

            entity.OwnsMany(o => o.Lines, lines => {
                lines.ToTable("order_lines");
                lines.WithOwner().HasForeignKey("OrderId");
                lines.Property<int>("Id").ValueGeneratedOnAdd(); // ensure key generation
                lines.HasKey("Id");
            });
        });
    }
}

public sealed class Order {
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public List<OrderLine> Lines { get; } = new();

    public static Order Create(Guid customerId, IEnumerable<(Guid productId, int qty, decimal price)> items) {
        var order = new Order { CustomerId = customerId };
        foreach (var (productId, qty, price) in items) {
            order.Lines.Add(new OrderLine(productId, qty, price));
        }
        return order;
    }
}

public sealed record OrderLine(Guid ProductId, int Quantity, decimal UnitPrice);
