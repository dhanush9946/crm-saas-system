using CRM.Domain.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Persistence.Configurations.Identity;

public sealed class ExternalLoginConfiguration
    : IEntityTypeConfiguration<ExternalLogin>
{
    public void Configure(EntityTypeBuilder<ExternalLogin> builder)
    {
        // Table Name
        builder.ToTable("ExternalLogins");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Provider)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ProviderUserId)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(320);


        // Relationship
        builder.HasOne(x => x.User)
            .WithMany(x => x.ExternalLogins)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique Index
        builder.HasIndex(x => new
        {
            x.TenantId,
            x.Provider,
            x.ProviderUserId
        })
        .IsUnique();

        // Optional Query Index
        builder.HasIndex(x => new
        {
            x.TenantId,
            x.UserId
        });
    }
}