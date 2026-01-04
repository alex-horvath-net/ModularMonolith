using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Business.ApplicationUsers.Member.Billing.Infrastructure.Data.Models;

namespace Business.ApplicationUsers.Member.Billing.Infrastructure.Data;

public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options) {
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasDefaultSchema("billing");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}