using AuthenticationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticationService.Infrastructure.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Email).IsUnique();

        builder
            .Property(u => u.Email)
            .HasMaxLength(255);

        builder
            .Property(u => u.PasswordHash)
            .HasMaxLength(1024);

        builder
            .Property(u => u.Name)
            .HasMaxLength(128);
    }
}
