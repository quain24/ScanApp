using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Helpers.EF_Queryable;
using ScanApp.Application.Common.Interfaces;

namespace ScanApp.Application.Common.Installers
{
    public static class ApplicationServiceInstaller
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IRecurrenceCheck, RecurrenceCheck>();

            return services;
        }
    }
}