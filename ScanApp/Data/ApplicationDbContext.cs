using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace ScanApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser>().ToTable("Users", "sca");
            builder.Entity<IdentityRole>().ToTable("Roles", "sca");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "sca");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "sca");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "sca");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "sca");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("UserRoleClaims", "sca");
        }
    }
}
