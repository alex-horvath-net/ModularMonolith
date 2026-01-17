using Experts.OrderExpert.Shared.Infrastructure.Data.Models;
using Experts.OrderExpert.Shared.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace Experts.OrderExpert.Shared.Infrastructure.Data;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options) {
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var type = typeof(OrdersDbContext);
        modelBuilder.ApplyConfigurationsFromAssembly(type.Assembly, x => x.Namespace!.StartsWith(type.Namespace!) );
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);
        if (optionsBuilder.IsConfigured) {
            var seeder = new DataSeeder(this);
            seeder.Seed();
        }
    }
}
