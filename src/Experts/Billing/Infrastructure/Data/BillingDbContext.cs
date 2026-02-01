using Experts.Billing.Infrastructure.Data.Models;
using Experts.OrderExpert.Common.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experts.Billing.Infrastructure.Data;

public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options) {
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var dbAssembly = typeof(OrdersDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(
            dbAssembly, 
            type => type.Namespace!.StartsWith(dbAssembly.GetName().Name!, StringComparison.InvariantCultureIgnoreCase));
    }
}