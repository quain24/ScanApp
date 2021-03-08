using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ScanApp.Store.Features.Middleware;

namespace ScanApp.Common.Installers
{
    public static class FluxorStateManagementInstaller
    {
        /// <summary>
        /// Adds Fluxor state management framework and configures all relative options / add-ons (Like redux dev tools to be used in browser)
        /// </summary>
        public static IServiceCollection AddFluxorStateManagement(this IServiceCollection services)
        {
            services.AddFluxor(options =>
            {
                options.ScanAssemblies(Assembly.GetExecutingAssembly())
                    .AddMiddleware<LoggingMiddleware>();
                options.UseReduxDevTools();
            });

            return services;
        }
    }
}