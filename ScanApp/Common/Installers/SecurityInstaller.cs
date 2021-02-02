using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Entities;
using ScanApp.Areas.Identity;
using ScanApp.Infrastructure.Persistence;
using System;

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

            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();

            return services;
        }
    }
}