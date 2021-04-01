using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Common.Interfaces;
using ScanApp.Common.Services;
using ScanApp.Infrastructure.Identity;
using ScanApp.Infrastructure.Services;

namespace ScanApp.Infrastructure.Common.Installers
{
    public static class ServiceInstaller
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddTransient<IScopedMediator, ScopedMediator>();
            services.AddTransient<IUserManager, UserManagerService>();
            services.AddTransient<IRoleManager, RoleManagerService>();
            services.AddTransient<IUserInfo, UserInfoService>();
            services.AddTransient<ILocationManager, LocationManagerService>();
            services.AddSingleton<IDateTime, DateTimeService>();
            services.AddScoped<IInitialDataSeeder, InitialDataSeeder>();

            return services;
        }
    }
}