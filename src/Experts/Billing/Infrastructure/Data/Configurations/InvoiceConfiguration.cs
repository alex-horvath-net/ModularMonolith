using Experts.Billing.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Experts.Billing.Infrastructure.Data.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice> {
    public void Configure(EntityTypeBuilder<Invoice> builder) {
        builder.ToTable("Invoices", "billing", tb => {
            tb.HasComment("Billing invoices");
            tb.HasCheckConstraint("CK_Invoices_Total_NonNegative", "[Total] >= 0");
            tb.HasCheckConstraint("CK_Invoices_OrderId_NotEmpty", "[OrderId] <> '00000000-0000-0000-0000-000000000000'");
            tb.HasCheckConstraint("CK_Invoices_CustomerId_NotEmpty", "[CustomerId] <> '00000000-0000-0000-0000-000000000000'");
        });

        builder.HasKey(i => i.Id);

        builder.HasIndex(i => i.CustomerId)
               .HasDatabaseName("IX_Invoices_CustomerId");

        builder.HasIndex(i => i.OrderId)
               .HasDatabaseName("IX_Invoices_OrderId");

        builder.Property(i => i.Total)
               .HasPrecision(19, 4);

        builder.Property(i => i.CreatedAtUtc)
               .HasColumnType("datetime2(3)")
               .HasDefaultValueSql("SYSUTCDATETIME()")
               .ValueGeneratedOnAdd();
    }
}