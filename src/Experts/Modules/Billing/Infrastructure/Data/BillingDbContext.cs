using Business.Modules.Billing.Infrastructure.Data.Models;
using Business.Modules.OrderExpert.Common.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Business.Modules.Billing.Infrastructure.Data;

public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options) {
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var dbAssembly = typeof(OrdersDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(
            dbAssembly,
            type => type.Namespace!.StartsWith(dbAssembly.GetName().Name!, StringComparison.InvariantCultureIgnoreCase));
    }
}