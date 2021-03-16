using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> builder)
        {
            builder.ToTable("ClaimsSource", "sca");
            builder.HasAlternateKey(c => new { c.Type, c.Value });
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.Property(c => c.Type).IsRequired();
            builder.Property(c => c.Value).IsRequired();

            builder.HasMany<IdentityRoleClaim<string>>()
                .WithOne()
                .HasPrincipalKey(c => new { c.Type, c.Value })
                .HasForeignKey(c => new { c.ClaimType, c.ClaimValue })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<IdentityUserClaim<string>>()
                .WithOne()
                .HasPrincipalKey(c => new { c.Type, c.Value })
                .HasForeignKey(c => new { c.ClaimType, c.ClaimValue })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}