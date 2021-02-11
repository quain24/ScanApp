using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Infrastructure.Services;

namespace ScanApp.Common.Installers
{
    public static class ScopedMediatorInstaller
    {
        /// <summary>
        /// For Blazor server a scoped version of MediatR is needed<br/>
        /// Thanks to this no more problems with scopes and DbContexts <br/>
        /// should happen inside request handlers
        /// </summary>
        public static IServiceCollection AddScopedMediator(this IServiceCollection services)
        {
            services.AddTransient<IScopedMediator, ScopedMediator>();
            return services;
        }
    }
}