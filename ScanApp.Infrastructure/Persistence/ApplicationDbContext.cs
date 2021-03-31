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

        public DbSet<Location> Locations { get; set; }
        public DbSet<UserLocation> UserLocations { get; set; }
        public DbSet<Claim> ClaimsSource { get; set; }

        public DbSet<SparePart> SpareParts { get; set; }
        public DbSet<SparePartType> SparePartTypes { get; set; }
        public DbSet<SparePartStoragePlace> StoragePlaces { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // small configurations here - no point in extracting one liners to separate files
            builder.Entity<IdentityRole>().ToTable("Roles", "sca");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "sca");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "sca");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "sca");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "sca");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("UserRoleClaims", "sca");

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}