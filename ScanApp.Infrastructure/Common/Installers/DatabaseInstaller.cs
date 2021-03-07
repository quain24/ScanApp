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
        public static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration, bool isDevelopment = false)
        {
            services.AddAspSecurityDatabase(configuration, isDevelopment);

            return services;
        }

        /// <summary>
        /// Database setup for Microsoft implementation of Identity service in ASP.CORE
        /// </summary>
        private static IServiceCollection AddAspSecurityDatabase(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            var sqlConfiguration = new Action<DbContextOptionsBuilder>(options =>
            {
                if (isDevelopment)
                    options.EnableSensitiveDataLogging();
                options.UseSqlServer(configuration.GetConnectionString(AspSecurityDbConnectionStringName),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                        sqlOptions.CommandTimeout(60);
                    });
            });

            // Registration for direct injection for UserManager and similar.
            // Registering it this way enables automatic lifetime management (no need to dispose)
            services.AddDbContext<ApplicationDbContext>(options => sqlConfiguration(options), optionsLifetime: ServiceLifetime.Singleton);
            services.AddScoped<IApplicationDbContext>(sp => sp.GetService<ApplicationDbContext>());

            // Registration for blazor to handle concurrency in components - context created by this factory requires manual dispose
            services.AddDbContextFactory<ApplicationDbContext>(options => sqlConfiguration(options));

            // Nicer code, but requires manual use of Dispose()
            //services.AddScoped<IApplicationDbContext>(p =>
            //    p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
            //        .CreateDbContext());
            //services.AddScoped(p => p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
            //        .CreateDbContext());

            return services;
        }
    }
}