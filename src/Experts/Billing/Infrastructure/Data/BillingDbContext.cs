using Experts.Billing.Infrastructure.Data.Models;
using Experts.OrderExpert.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experts.Billing.Infrastructure.Data;

public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options) {
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var type = typeof(OrdersDbContext);
        modelBuilder.ApplyConfigurationsFromAssembly(type.Assembly, x => x.Namespace!.StartsWith(type.Namespace!));
    }
}