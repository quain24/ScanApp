using Globals;
using Microsoft.AspNetCore.Authorization;

namespace ScanApp.Areas.Identity
{
    /// <summary>
    /// In this class all of the security policies should be configured.<br/>
    /// Those policies will be installed automatically on startup.
    /// <para>Note, that the policies configured here ARE NOT calling <see cref="AuthorizationPolicyBuilder.Build"/></para>
    /// <para>Note, when configuring policies, that claim VALUES and Role names are case sensitive</para>
    /// </summary>
    public static class Policies
    {
        // You CANNOT use this policy in your controllers or pages.
        /// <summary>
        /// <strong>Not to be used by developer directly!</strong><br/>
        /// This is a auto policy so that automatic policy searching and default fallback will work
        /// </summary>
        /// <param name="builder"></param>
        internal static void PolicyConfigurationFailedFallback(AuthorizationPolicyBuilder builder)
            => builder.RequireAssertion(_ => false);

        public static void LocationMustBeSady(AuthorizationPolicyBuilder builder)
            => builder.RequireAuthenticatedUser()
                .RequireClaim(ClaimTypes.Location, "DF5BBE28-688E-4DA8-8E79-3C1D9C75CFAA");
    }
}