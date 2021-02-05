using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Infrastructure.Persistence;
using System;

namespace ScanApp.Infrastructure.Common.Installers
{
    public static class DatabaseInstaller
    {
        private static string AspSecurityDbConnectionStringName => "TestConnection";

        /// <summary>
        /// Adds all databases, DbContexts and similar, used in this project to DI container. Receives connection strings from <paramref name="configuration"/>
        /// </summary>
        public static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAspSecurityDatabase(configuration);

            return services;
        }

        /// <summary>
        /// Database setup for Microsoft implementation of Identity service in ASP.CORE
        /// </summary>
        private static IServiceCollection AddAspSecurityDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // Registration for blazor to handle concurrency in components
            services.AddDbContextFactory<ApplicationDbContext>(b =>
            {
                b.UseSqlServer(configuration.GetConnectionString(AspSecurityDbConnectionStringName),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });

            // Registration for direct injection for UserManager and similar.
            // Registering it this way enables automatic lifetime management (no need to dispose)
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(AspSecurityDbConnectionStringName),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            }, optionsLifetime: ServiceLifetime.Singleton);
            services.AddScoped<IApplicationDbContext>(sp => sp.GetService<ApplicationDbContext>());

            // Nicer code, but not sure if wont cause need for disposable implementation
            //services.AddScoped<IApplicationDbContext>(p =>
            //    p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
            //        .CreateDbContext());
            //services.AddScoped(p => p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
            //        .CreateDbContext());

            return services;
        }
    }
}