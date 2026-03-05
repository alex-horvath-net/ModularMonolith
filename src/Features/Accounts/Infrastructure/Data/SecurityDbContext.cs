using Microsoft.EntityFrameworkCore;

namespace Features.Accounts.Infrastructure.Data;

public sealed class SecurityDbContext(DbContextOptions<SecurityDbContext> options) : DbContext(options) {
    public DbSet<Models.Account> Accounts => Set<Models.Account>();
    public DbSet<Models.Role> Roles => Set<Models.Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var type = typeof(SecurityDbContext);
        modelBuilder.HasDefaultSchema("accounts");
        modelBuilder.ApplyConfigurationsFromAssembly(type.Assembly, x => x.Namespace!.StartsWith(type.Namespace!, StringComparison.Ordinal));
    }
}
