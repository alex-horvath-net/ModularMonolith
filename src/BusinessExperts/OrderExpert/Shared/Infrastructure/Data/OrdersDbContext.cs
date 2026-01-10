using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Experts.OrderExpert.Shared.Infrastructure.Data;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options) {
    public DbSet<Models.Order> Orders => Set<Models.Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasDefaultSchema("orders");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
