using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace ScanApp.Common.Installers
{
    /// <summary>
    /// Provides <see cref="Fluxor"/> installer method that can be used when configuring services in <see cref="Startup"/>
    /// </summary>
    public static class FluxorStateManagementInstaller
    {
        /// <summary>
        /// Adds Fluxor state management framework and configures all relative options / add-ons (Like redux dev tools to be used in browser)
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> instance upon which configuration will be performed</param>
        /// <returns>Configured Service collection</returns>
        public static IServiceCollection AddFluxorStateManagement(this IServiceCollection services)
        {
            services.AddFluxor(options =>
            {
                options.ScanAssemblies(typeof(Startup).Assembly);
                options.UseReduxDevTools();
            });
            return services;
        }
    }
}