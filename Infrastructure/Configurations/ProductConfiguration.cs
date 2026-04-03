using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Namespace.Infrastructure.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(prop => prop.ProductPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(prop => prop.ProductName)
            .IsRequired()
            .HasMaxLength(32);
    }
}