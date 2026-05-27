using CRM.Domain.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.TenantId, x.UserId, x.RoleId })
            .IsUnique();

        builder.HasOne(x => x.User)  // <-- Map the property explicitly
        .WithMany(u => u.UserRoles)
        .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Role)  // <-- Map the property explicitly
            .WithMany(r => r.UserRoles)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}