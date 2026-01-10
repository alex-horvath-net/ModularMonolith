using System.Reflection;
using Experts.BillingBusinessExpert.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Experts.BillingBusinessExpert.Infrastructure.Data;

public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options) {
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasDefaultSchema("billing");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}