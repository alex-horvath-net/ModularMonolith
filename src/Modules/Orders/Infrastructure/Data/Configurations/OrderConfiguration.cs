using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Infrastructure.Data.Models;

namespace Orders.Infrastructure.Data.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order> {
    public void Configure(EntityTypeBuilder<Order> e) {
        e.ToTable("Orders");
        e.HasKey(o => o.Id);
        e.OwnsMany(o => o.Lines, b => {
            b.ToTable("OrderLines");
            b.WithOwner().HasForeignKey("OrderId");
            b.Property<Guid>("Id"); 
            b.HasKey("Id");
            b.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        });

        e.Property<byte[]>("RowVersion").IsRowVersion();

        e.Navigation(o => o.Lines)
         .HasField("_lines")
         .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
