using AzureBasicNet6.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzureBasicNet6.Database.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(t => t.ProductId);
            //builder.Property(t => t.ImageName)
            //    .HasMaxLength(100);
                //.IsRequired();
            //builder.Property(t => t.ImagePath)
            //    .HasMaxLength(250);
                //.IsRequired();
            builder.Property(t => t.Description)
                .HasMaxLength(500)
                .IsRequired();
            builder.Property(t => t.Name)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
