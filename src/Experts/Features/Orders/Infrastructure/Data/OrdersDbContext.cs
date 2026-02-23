using Core.Features.Orders.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.Orders.Infrastructure.Data;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options) {
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var dbAssembly = typeof(OrdersDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(
            dbAssembly,
            type => type.Namespace!.StartsWith(type.Namespace!, StringComparison.InvariantCultureIgnoreCase));
    }
}
