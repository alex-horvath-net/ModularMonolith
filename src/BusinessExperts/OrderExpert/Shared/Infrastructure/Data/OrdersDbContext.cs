using Experts.OrderExpert.Shared.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Experts.OrderExpert.Shared.Infrastructure.Data;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options) {
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var type = typeof(OrdersDbContext);
        modelBuilder.ApplyConfigurationsFromAssembly(type.Assembly, x => x.Namespace!.StartsWith(type.Namespace!) );
    }
}
