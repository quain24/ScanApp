using Microsoft.AspNetCore.Authorization;

namespace ScanApp.Areas.Identity
{
    /// <summary>
    /// In this class all of the security policies should be configured.<br/>
    /// Those policies will be installed automatically on startup.
    /// <para>Note, that the policies configured here ARE NOT calling <see cref="AuthorizationPolicyBuilder.Build"/></para>
    /// <para>Note, when configuring policies, that claim VALUES are case sensitive</para>
    /// </summary>
    public static class Policies
    {
        // You CANNOT use this policy in your controllers or pages.
        internal static void PolicyConfigurationFailedFallback(AuthorizationPolicyBuilder builder)
            => builder.RequireAssertion(_ => false);

        public static void LocationMustBePoznan(AuthorizationPolicyBuilder builder)
            => builder.RequireAuthenticatedUser()
                .RequireClaim("location", "Poznan");
    }
}