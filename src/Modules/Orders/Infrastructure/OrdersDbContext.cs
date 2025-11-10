using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orders.Infrastructure;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options) {
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Order>(e => {
            e.HasKey(o => o.Id);

            // Configure owned collection
            e.OwnsMany(o => o.Lines, b => {
                b.WithOwner().HasForeignKey("OrderId");
                b.Property<Guid>("Id"); // Shadow key for owned entity instances
                b.HasKey("Id");
            });

            // Map backing field for collection to allow EF to populate it
            e.Navigation(o => o.Lines)
             .HasField("_lines")
             .UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}

// Changed from positional record (EF couldn't bind navigation in ctor) to standard class with EF-friendly constructor
public sealed class Order {
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }

    private readonly List<OrderLine> _lines = new();
    public IReadOnlyList<OrderLine> Lines => _lines;

    // Parameterless constructor required by EF Core
    private Order() { }

    private Order(Guid id, Guid customerId) {
        Id = id;
        CustomerId = customerId;
    }

    public static Order Create(Guid customerId, IEnumerable<(Guid productId, int quantity, decimal unitPrice)> lines) {
        var order = new Order(Guid.NewGuid(), customerId);
        foreach (var (productId, quantity, unitPrice) in lines) {
            order._lines.Add(new OrderLine(productId, quantity, unitPrice));
        }
        return order;
    }
}

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
