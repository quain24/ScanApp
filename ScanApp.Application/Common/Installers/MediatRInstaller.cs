using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Behaviors;

namespace ScanApp.Application.Common.Installers
{
    public static class MediatRInstaller
    {
        /// <summary>
        /// All <see cref="MediatR.Mediator"/> DI Configurations, including behaviors that are setup here.
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(typeof(MediatRInstaller));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TimingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));

            return services;
        }
    }
}