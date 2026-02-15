using Business.Experts.OrderExpert.Common.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Business.Experts.OrderExpert.Common.Infrastructure.Data.Configurations;

public sealed class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine> {
    public void Configure(EntityTypeBuilder<OrderLine> builder) {
        builder.ToTable("OrderLines", "orders", tb => {
            tb.HasCheckConstraint("CK_OrderLines_Quantity_Positive", "[Quantity] > 0");
            tb.HasCheckConstraint("CK_OrderLines_UnitPrice_NonNegative", "[UnitPrice] >= 0");
        });

        builder.HasKey(l => l.Id);
        builder.Property<Guid>("OrderId").IsRequired();
        builder.HasIndex("OrderId").HasDatabaseName("IX_OrderLines_OrderId");

        builder.Property(l => l.UnitPrice).HasPrecision(19, 4);
        builder.Property(l => l.Quantity).IsRequired();
    }
}
