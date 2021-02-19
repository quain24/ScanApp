using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace ScanApp.Common.Installers
{
    public static class MudBlazorInstaller
    {
        public static IServiceCollection AddMudBlazor(this IServiceCollection services)
        {
            services.AddMudServices();

            return services;
        }
    }
}