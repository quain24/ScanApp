using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ScanApp.Application.Common.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ScanApp.Infrastructure.Identity
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user).ConfigureAwait(false);
            var identity = principal.Identity as ClaimsIdentity;

            // Add custom claims / other sources claims
            identity?.AddClaims(new[]
            {
                new Claim("Location", user.LocationId.ToString())
            });

            // Select only distinct user and role claims
            var claims = identity?.Claims.ToList();
            var distinctClaims = identity
                ?.Claims
                .GroupBy(c =>
                    c.Type, (_, groupedByName) =>
                        groupedByName.GroupBy(c =>
                            c.Value, (_, groupedByValue) => groupedByValue.First()))
                .SelectMany(c => c)
                .ToList();

            foreach (var claim in claims ?? new List<Claim>(0))
            {
                identity?.RemoveClaim(claim);
            }

            identity?.AddClaims(distinctClaims);

            return principal;
        }
    }
}