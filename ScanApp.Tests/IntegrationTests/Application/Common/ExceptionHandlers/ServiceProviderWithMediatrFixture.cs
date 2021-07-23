using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Installers;

namespace ScanApp.Tests.IntegrationTests.Application.Common.ExceptionHandlers
{
    public class ServiceProviderWithMediatrFixture
    {
        private ServiceProvider _provider;
        public ServiceProvider Provider => _provider ??= ServiceCollection.BuildServiceProvider();

        private ServiceCollection _serviceCollection;
        public ServiceCollection ServiceCollection
        {
            get
            {
                if (_serviceCollection is null)
                {
                    _serviceCollection = new ServiceCollection();
                    ConfigureServices(_serviceCollection);
                }
                return _serviceCollection;
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(MediatRInstaller));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
            services.AddTransient<IRequestHandler<ExceptionHandlerFixtures.Command, Result>, ExceptionHandlerFixtures.GenericHandler>();
        }
    }
}