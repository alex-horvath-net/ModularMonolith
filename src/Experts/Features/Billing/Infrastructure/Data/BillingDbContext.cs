using Core.Features.Billing.Infrastructure.Data.Models;
using Core.Features.Orders.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.Billing.Infrastructure.Data;

public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options) {
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var dbAssembly = typeof(OrdersDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(
            dbAssembly,
            type => type.Namespace!.StartsWith(dbAssembly.GetName().Name!, StringComparison.InvariantCultureIgnoreCase));
    }
}