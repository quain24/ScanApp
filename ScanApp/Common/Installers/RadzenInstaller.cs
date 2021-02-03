using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace ScanApp.Common.Installers
{
    public static class RadzenInstaller
    {
        /// <summary>
        /// Configures and registers all additional services required for Radzen to work, like Notifications or dialogs
        /// </summary>
        public static IServiceCollection AddRadzenConfiguration(this IServiceCollection services)
        {
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();

            return services;
        }
    }
}