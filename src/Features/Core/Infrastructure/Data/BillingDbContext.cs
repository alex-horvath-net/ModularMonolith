using Billing.Core.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Billing.Core.Infrastructure.Data;

public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options) {
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var dbAssembly = typeof(BillingDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(
            dbAssembly,
            type => type.Namespace!.StartsWith(dbAssembly.GetName().Name!, StringComparison.InvariantCultureIgnoreCase));
    }
}