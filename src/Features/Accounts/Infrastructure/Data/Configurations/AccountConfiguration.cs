using Features.Accounts.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Features.Accounts.Infrastructure.Data.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account> {
    public void Configure(EntityTypeBuilder<Account> builder) {
        builder.ToTable("Accounts", "accounts", tb => {
            tb.HasComment("Local accounts");
            tb.HasCheckConstraint("CK_Accounts_Id_NotEmpty", "[Id] <> '00000000-0000-0000-0000-000000000000'");
        });

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
               .ValueGeneratedOnAdd();

        builder.Property(a => a.UserName)
               .HasMaxLength(256)
               .IsRequired();

        builder.Property(a => a.UserNameNormalized)
               .HasMaxLength(256)
               .IsRequired();

        builder.Property(a => a.Email)
               .HasMaxLength(320)
               .IsRequired();

        builder.Property(a => a.EmailNormalized)
               .HasMaxLength(320)
               .IsRequired();

        builder.Property(a => a.PasswordHash)
               .HasMaxLength(512)
               .IsRequired();

        builder.Property(a => a.PasswordChangedAtUtc)
               .HasColumnType("datetime2(3)");

        builder.Property(a => a.IsLocked)
               .IsRequired();

        builder.Property(a => a.FailedAccessCount)
               .HasDefaultValue(0);

        builder.Property(a => a.LockoutEndUtc)
               .HasColumnType("datetime2(3)");

        builder.Property(a => a.LastLoginAtUtc)
               .HasColumnType("datetime2(3)");

        builder.Property(a => a.CreatedAtUtc)
               .HasColumnType("datetime2(3)")
               .HasDefaultValueSql("SYSUTCDATETIME()")
               .ValueGeneratedOnAdd();

        builder.Property(a => a.UpdatedAtUtc)
               .HasColumnType("datetime2(3)")
               .HasDefaultValueSql("SYSUTCDATETIME()")
               .ValueGeneratedOnAdd();

        builder.Property(a => a.CreatedBy)
               .HasMaxLength(128);

        builder.Property(a => a.UpdatedBy)
               .HasMaxLength(128);

        builder.Property(a => a.IsDeleted)
               .HasDefaultValue(false);

        builder.Property(a => a.DeletedAtUtc)
               .HasColumnType("datetime2(3)");

        builder.Property(a => a.DeletedBy)
               .HasMaxLength(128);

        builder.Property(a => a.RowVersion)
               .IsRowVersion();

        builder.HasIndex(a => a.EmailNormalized)
               .IsUnique()
               .HasDatabaseName("IX_Accounts_Email_Normalized");

        builder.HasIndex(a => a.UserNameNormalized)
               .IsUnique()
               .HasDatabaseName("IX_Accounts_UserName_Normalized");

        builder.HasIndex(a => a.IsDeleted)
               .HasDatabaseName("IX_Accounts_IsDeleted");

        builder.HasIndex(a => a.IsLocked)
               .HasDatabaseName("IX_Accounts_IsLocked");

        builder.HasMany(a => a.Roles)
               .WithMany()
               .UsingEntity<Dictionary<string, object>>(
                   "AccountRole",
                   role => role.HasOne<Role>()
                               .WithMany()
                               .HasForeignKey("RoleId")
                               .OnDelete(DeleteBehavior.Cascade),
                   account => account.HasOne<Account>()
                                     .WithMany()
                                     .HasForeignKey("AccountId")
                                     .OnDelete(DeleteBehavior.Cascade),
                   join => {
                       join.ToTable("AccountRoles", "accounts");
                       join.HasKey("AccountId", "RoleId");
                       join.HasIndex("AccountId").HasDatabaseName("IX_AccountRoles_AccountId");
                       join.HasIndex("RoleId").HasDatabaseName("IX_AccountRoles_RoleId");
                   });

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
