using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Infrastructure.Persistence;
using System;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Interfaces;

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
            IServiceCollection serviceCollections = services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(AspSecurityDbConnectionStringName),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });

            // Adds abstraction so this can be injected into Command / Query
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            return services;
        }
    }
}