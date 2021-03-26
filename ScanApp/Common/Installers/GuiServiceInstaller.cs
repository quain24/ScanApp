using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Services;

namespace ScanApp.Common.Installers
{
    public static class GuiServiceInstaller
    {
        public static IServiceCollection AddGuiServices(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}