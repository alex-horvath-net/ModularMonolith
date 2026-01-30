using Microsoft.EntityFrameworkCore;

namespace Experts.SecurityOfficer.Shared.Infrastructure.Data;

public sealed class SecurityOfficerDbContext(DbContextOptions<SecurityOfficerDbContext> options) : DbContext(options) {
    public DbSet<Models.Account> Accounts => Set<Models.Account>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var type = typeof(SecurityOfficerDbContext);
        modelBuilder.ApplyConfigurationsFromAssembly(type.Assembly, x => x.Namespace!.StartsWith(type.Namespace!));
    }
}
