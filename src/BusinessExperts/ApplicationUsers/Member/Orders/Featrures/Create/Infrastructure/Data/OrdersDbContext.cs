using System.Reflection;
using Business.ApplicationUsers.Member.Orders.Featrures.Create.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.ApplicationUsers.Member.Orders.Featrures.Create.Infrastructure.Data;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options) {
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasDefaultSchema("orders");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
