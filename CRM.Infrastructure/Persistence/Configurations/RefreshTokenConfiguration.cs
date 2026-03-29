

using CRM.Domain.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration:IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TokenHash)
                .IsRequired();

            builder.Property(x => x.DeviceId)
                .HasMaxLength(200);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(400);

            builder.Property(x => x.IpAddress)
                .HasMaxLength(45);

           
            builder.HasIndex(x => new { x.TenantId, x.UserId, x.ExpiresAtUtc });

            
            builder.HasIndex(x => x.TokenHash)
                .IsUnique();

            // Relationships
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
