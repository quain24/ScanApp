using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Areas.Identity
{
    /// <summary>
    /// Provides a way to systematically check if currently logged user is still allowed to browse page.<br/>
    /// Uses built-in Security Stamp functionality of <see cref="Application.Common.Entities.ApplicationUser"/>.<br/>
    /// <para>If user security clearance has been modified, his security stamp will change.<br/></para>
    /// </summary>
    /// <typeparam name="TUser">Type of user</typeparam>
    public class RevalidatingIdentityAuthenticationStateProvider<TUser>
        : RevalidatingServerAuthenticationStateProvider where TUser : class
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IdentityOptions _options;

        /// <summary>
        /// Initializes new instance of <see cref="RevalidatingIdentityAuthenticationStateProvider{TUser}"/>
        /// </summary>
        /// <param name="loggerFactory">Used to create new loggers inside base class</param>
        /// <param name="scopeFactory">Scope factory used to create fresh scope for each revalidation request</param>
        /// <param name="optionsAccessor">Source of default <see cref="Claim"/> types</param>
        public RevalidatingIdentityAuthenticationStateProvider(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory scopeFactory,
            IOptions<IdentityOptions> optionsAccessor)
            : base(loggerFactory)
        {
            _scopeFactory = scopeFactory;
            _options = optionsAccessor.Value;
        }

        /// <summary>
        /// Interval on which each logged user will be revalidated
        /// </summary>
        protected override TimeSpan RevalidationInterval => TimeSpan.FromSeconds(10);

        protected override async Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            // Get the user manager from a new scope to ensure it fetches fresh data
            var scope = _scopeFactory.CreateScope();
            try
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
                return await ValidateSecurityStampAsync(userManager, authenticationState.User);
            }
            finally
            {
                if (scope is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else
                {
                    scope.Dispose();
                }
            }
        }

        private async Task<bool> ValidateSecurityStampAsync(UserManager<TUser> userManager, ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            if (user == null)
                return false;

            if (!userManager.SupportsUserSecurityStamp)
                return true;

            var principalStamp = principal.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);
            var userStamp = await userManager.GetSecurityStampAsync(user);
            return principalStamp == userStamp;
        }
    }
}