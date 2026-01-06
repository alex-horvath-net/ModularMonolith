using ApplicationUsers.Member.BusinessExperts.OrderBusinessExpert.CreateOrderWorkFlow.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessExperts.Order.CreateOrderWorkFlow.Infrastructure.Data.Configurations;


public sealed class OrderConfiguration : IEntityTypeConfiguration<Order> {
    public void Configure(EntityTypeBuilder<Order> builder) {
        builder.ToTable("Orders", "orders", tb => {
            tb.HasCheckConstraint("CK_Orders_CustomerId_NotEmpty", "[CustomerId] <> '00000000-0000-0000-0000-000000000000'");
            tb.HasCheckConstraint("CK_Orders_UpdatedUtc_NotBeforeCreatedUtc", "[UpdatedUtc] >= [CreatedUtc]");
        });

        builder.HasKey(o => o.Id);
        builder.HasIndex(o => o.CustomerId).HasDatabaseName("IX_Orders_CustomerId");

        builder.Property<byte[]>("RowVersion").IsRowVersion();
        builder.Property<DateTime>("CreatedUtc").HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()").ValueGeneratedOnAdd();
        builder.Property<DateTime>("UpdatedUtc").HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()").ValueGeneratedOnAddOrUpdate();

        builder.OwnsMany(o => o.Lines, owned => {
            owned.ToTable("OrderLines", "orders", tb => {
                tb.HasCheckConstraint("CK_OrderLines_Quantity_Positive", "[Quantity] > 0");
                tb.HasCheckConstraint("CK_OrderLines_UnitPrice_NonNegative", "[UnitPrice] >= 0");
            });

            owned.WithOwner().HasForeignKey("OrderId");
            owned.Property<Guid>("Id");
            owned.HasKey("Id");
            owned.Property(l => l.UnitPrice).HasPrecision(19, 4);
            owned.Property(l => l.Quantity).IsRequired();
            owned.HasIndex("OrderId").HasDatabaseName("IX_OrderLines_OrderId");
        });

        builder.Navigation(o => o.Lines).HasField("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}