using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.HasIndex(o => o.CreatedAtUtc);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.UserId);

        builder
            .Property(o => o.Status)
            .HasConversion<string>()
            .IsConcurrencyToken();

        builder
            .OwnsOne(
                o => o.Position,
                p =>
                {
                    p.Property(p => p.Item).HasMaxLength(256);
                });
    }
}
