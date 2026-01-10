using System.Reflection;
using Experts.OrderBusinessExpert.BusinessWorkFlows.Shared.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Experts.OrderBusinessExpert.BusinessWorkFlows.Shared.Infrastructure.Data;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options) {
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasDefaultSchema("orders");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
