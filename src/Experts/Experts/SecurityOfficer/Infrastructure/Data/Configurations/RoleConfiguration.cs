using Business.Experts.SecurityOfficer.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Business.Experts.SecurityOfficer.Infrastructure.Data.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role> {
    public void Configure(EntityTypeBuilder<Role> builder) {
        builder.ToTable("Roles", "security", tb => {
            tb.HasComment("Security officer roles");
            tb.HasCheckConstraint("CK_Roles_Id_NotEmpty", "[Id] <> '00000000-0000-0000-0000-000000000000'");
            tb.HasCheckConstraint("CK_Roles_Name_NotEmpty", "LEN([Name]) > 0");
        });

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
               .ValueGeneratedOnAdd();

        builder.Property(r => r.Name)
               .HasMaxLength(128)
               .IsRequired();

        builder.Property(r => r.NormalizedName)
               .HasMaxLength(128)
               .IsRequired();

        builder.Property(r => r.CreatedAtUtc)
               .HasColumnType("datetime2(3)")
               .HasDefaultValueSql("SYSUTCDATETIME()")
               .ValueGeneratedOnAdd();

        builder.Property(r => r.UpdatedAtUtc)
               .HasColumnType("datetime2(3)")
               .HasDefaultValueSql("SYSUTCDATETIME()")
               .ValueGeneratedOnAdd();

        builder.Property(r => r.IsDeleted)
               .HasDefaultValue(false);

        builder.Property(r => r.DeletedAtUtc)
               .HasColumnType("datetime2(3)");

        builder.Property(r => r.DeletedBy)
               .HasMaxLength(128);

        builder.Property(r => r.RowVersion)
               .IsRowVersion();

        builder.HasIndex(r => r.NormalizedName)
               .IsUnique()
               .HasDatabaseName("IX_Roles_NormalizedName");

        builder.HasIndex(r => r.IsDeleted)
               .HasDatabaseName("IX_Roles_IsDeleted");

        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
