using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Radzen;
using DialogService = MudBlazor.DialogService;

namespace ScanApp.Common.Installers
{
    public static class RadzenInstaller
    {
        public static IServiceCollection AddRadzenBlazor(this IServiceCollection services)
        {
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();
            return services;
        }
    }
}
