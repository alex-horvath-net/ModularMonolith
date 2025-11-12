using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Infrastructure.Data.Models;

namespace Orders.Infrastructure.Data.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order> {
    public void Configure(EntityTypeBuilder<Order> e) {
        e.HasKey(o => o.Id);

        // Concurrency token (shadow property)
        e.Property<byte[]>("RowVersion").IsRowVersion();

        // Owned collection
        e.OwnsMany(o => o.Lines, b => {
            b.WithOwner().HasForeignKey("OrderId");
            b.Property<Guid>("Id"); // Shadow key for owned entity instances
            b.HasKey("Id");

            // Currency precision
            b.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        });

        // Backing field mapping for the collection
        e.Navigation(o => o.Lines)
         .HasField("_lines")
         .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
