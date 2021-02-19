using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ScanApp.Application.Common.Entities;
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
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[]
            {
                new Claim("Location", user.Location),
            });

            return principal;
        }
    }
}