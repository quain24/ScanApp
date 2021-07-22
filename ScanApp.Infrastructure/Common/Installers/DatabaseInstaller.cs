using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Infrastructure.Persistence;
using ScanApp.Infrastructure.Services;
using System;
using EntityFramework.Exceptions.SqlServer;

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
                    })
                    // Replace standard EF Core exception with more detailed ones. (EntityFramework.Exceptions package for SQL Server)
                    .UseExceptionProcessor();
            });

            services.AddDbContextFactory<ApplicationDbContext>(options => sqlConfiguration(options));
            services.AddSingleton<IContextFactory, AppDbContextFactory>(srv => new AppDbContextFactory(srv.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()));
            services.AddScoped<IApplicationDbContext>(p => p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
            services.AddDbContext<ApplicationDbContext>(options => sqlConfiguration(options), optionsLifetime: ServiceLifetime.Singleton);

            return services;
        }
    }
}