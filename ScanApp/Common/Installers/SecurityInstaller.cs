using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScanApp.Application.Common.Entities;
using ScanApp.Areas.Identity;
using ScanApp.Common.Extensions;
using ScanApp.Infrastructure.Identity;
using ScanApp.Infrastructure.Persistence;
using System;

namespace ScanApp.Common.Installers
{
    public static class SecurityInstaller
    {
        /// <summary>
        /// Configures all ASP Core Identity and security options / settings, such as password requirements or User / role managers
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="storeIdentityInMemory">If true, enables storing of user claims and roles on server side.<br/>
        /// This behavior will help when asp identity cookie will grow to large because of large amount of claims or roles assigned to user<br/>
        /// and browsers wont be able to handle it properly.
        /// <para><see lang="false"/> - DEFAULT -Standard behavior - cookie stores all claims and roles on client side</para>
        /// <para><see lang="true"/> - Cookie only points to store on server side</para>
        /// </param>
        public static IServiceCollection AddSecurityConfiguration(this IServiceCollection services, bool storeIdentityInMemory = false)
        {
            var identityOptions = new Action<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 3;

                // lockout setup
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                options.Lockout.MaxFailedAccessAttempts = 2;
                options.Lockout.AllowedForNewUsers = true;
            });

            var identityBuilder = storeIdentityInMemory
                ? services.SetUpIdentityToStoreDataInMemoryStore<ApplicationUser>(identityOptions)
                : services.AddDefaultIdentity<ApplicationUser>(identityOptions);

            identityBuilder
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddUserManager<UserManager<ApplicationUser>>()
                .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

            // Login page is displayed if user is not authorized - also on startup
            // Automatic policy registration from Policies.cs
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddSharedPolicies(services.BuildServiceProvider().GetService<ILogger<Startup>>());
            });

            // Enables immediate logout after refresh if user logged in on another session (zero interval is safe when using SignalR)
            // Guarantees update in roles and claims on each page refresh.
            services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.Zero);

            // Registers a service to refresh user authorization periodically - timespan is set inside of this service
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            return services;
        }

        private static IdentityBuilder SetUpIdentityToStoreDataInMemoryStore<TUser>(this IServiceCollection services, Action<IdentityOptions> options)
        where TUser : class
        {
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddIdentityCookies(o =>
            {
                o.ApplicationCookie.PostConfigure(cookie => cookie.SessionStore = new MemoryCacheTicketStore());
            });

            return services.AddIdentityCore<TUser>(o =>
                {
                    o.Stores.MaxLengthForKeys = 128;
                    options?.Invoke(o);
                })
                .AddDefaultUI()
                .AddDefaultTokenProviders();
        }
    }
}