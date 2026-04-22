using Microsoft.EntityFrameworkCore;

namespace Accounts.Core.Infrastructure.Data;

public sealed class AccountDbContext(DbContextOptions<AccountDbContext> options) : DbContext(options) {
    public DbSet<Models.Account> Accounts => Set<Models.Account>();
    public DbSet<Models.Role> Roles => Set<Models.Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var type = typeof(AccountDbContext);
        modelBuilder.HasDefaultSchema("accounts");
        modelBuilder.ApplyConfigurationsFromAssembly(type.Assembly, x => x.Namespace!.StartsWith(type.Namespace!, StringComparison.Ordinal));
    }
}
