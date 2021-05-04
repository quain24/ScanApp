using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Services;

namespace ScanApp.Common.Installers
{
    /// <summary>
    /// Provides convenient way to setup all services implementation from Blazor assembly to be used in <see cref="Startup"/>
    /// </summary>
    public static class GuiServiceInstaller
    {
        /// <summary>
        /// Adds all services that have their concrete implementations in Blazor assembly
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> instance upon which configuration will be performed</param>
        /// <returns>Configured Service collection</returns>
        public static IServiceCollection AddGuiServices(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}