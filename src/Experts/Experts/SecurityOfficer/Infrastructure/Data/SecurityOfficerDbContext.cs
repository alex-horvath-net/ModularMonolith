using Microsoft.EntityFrameworkCore;

namespace Business.Experts.SecurityOfficer.Infrastructure.Data;

public sealed class SecurityOfficerDbContext(DbContextOptions<SecurityOfficerDbContext> options) : DbContext(options) {
    public DbSet<Models.Account> Accounts => Set<Models.Account>();
    public DbSet<Models.Role> Roles => Set<Models.Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var type = typeof(SecurityOfficerDbContext);
        modelBuilder.HasDefaultSchema("security");
        modelBuilder.ApplyConfigurationsFromAssembly(type.Assembly, x => x.Namespace!.StartsWith(type.Namespace!, StringComparison.Ordinal));
    }
}
