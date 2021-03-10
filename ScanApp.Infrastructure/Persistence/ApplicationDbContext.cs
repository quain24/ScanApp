using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // small configurations here - no point in extracting one liners to separate files
            builder.Entity<IdentityRole>().ToTable("Roles", "sca");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "sca");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "sca");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "sca");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "sca");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("UserRoleClaims", "sca");

            builder.Entity<UserLocation>().ToTable("UserLocations", "sca");
        }
    }
}