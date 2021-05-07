using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;

namespace ScanApp.Common.Installers
{
    /// <summary>
    /// Provides convenient way to setup <see cref="MudBlazor"/> framework to be used in <see cref="Startup"/>
    /// </summary>
    public static class MudBlazorInstaller
    {
        /// <summary>
        /// Adds all <see cref="MudBlazor"/> necessary configuration to given <paramref name="services"/> instance<br/>
        /// Configures default options for <see cref="ISnackbar"/> instances throughout application
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> instance upon which configuration will be performed</param>
        /// <returns>Configured Service collection</returns>
        public static IServiceCollection AddMudBlazor(this IServiceCollection services)
        {
            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 5000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });

            return services;
        }
    }
}