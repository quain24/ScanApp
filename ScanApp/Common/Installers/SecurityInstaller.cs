using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Areas.Identity;
using ScanApp.Infrastructure.Persistence;
using System;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Infrastructure.Identity;

namespace ScanApp.Common.Installers
{
    public static class SecurityInstaller
    {
        /// <summary>
        /// Configures all ASP Core Identity and security options / settings, such as password requirements or User / role managers
        /// </summary>
        public static IServiceCollection AddSecurityConfiguration(this IServiceCollection services)
        {
            services.AddDefaultIdentity<ApplicationUser>(options =>
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
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddUserManager<UserManager<ApplicationUser>>();
            
            // Enables immediate logout after refresh if user logged in on another session (zero interval is safe when using SignalR)
            services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.Zero);

            // Login page is displayed if user is not authorized - also on startup
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            // Enables immediate logout after refresh if user logged in on another session (zero interval is safe when using SignalR)
            services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.Zero);

            // Registers a service to refresh user authorization periodically - timespan is set inside of this service
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            return services;
        }
    }
}