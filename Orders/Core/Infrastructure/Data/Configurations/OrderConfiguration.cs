using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Core.Infrastructure.Data.Models;

namespace Orders.Core.Infrastructure.Data.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order> {
    public void Configure(EntityTypeBuilder<Order> builder) {
        builder.ToTable("Orders", "orders", tb => {
            tb.HasCheckConstraint("CK_Orders_CustomerId_NotEmpty", "[CustomerId] <> '00000000-0000-0000-0000-000000000000'");
            tb.HasCheckConstraint("CK_Orders_UpdatedUtc_NotBeforeCreatedUtc", "[UpdatedUtc] >= [CreatedUtc]");
        });

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.CustomerId).HasDatabaseName("IX_Orders_CustomerId");

        builder.Property<byte[]>("RowVersion").IsRowVersion();
        builder.Property<DateTime>("CreatedUtc").HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()").ValueGeneratedOnAdd();
        builder.Property<DateTime>("UpdatedUtc").HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()").ValueGeneratedOnAddOrUpdate();

        builder.HasMany(x => x.Lines).WithOne().HasForeignKey("OrderId").OnDelete(DeleteBehavior.Cascade);

    }
}
