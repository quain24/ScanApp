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

        public ServiceProvider Provider
        {
            get
            {
                if (_provider is null)
                {
                    ConfigureServices(ServiceCollection);
                    _provider = ServiceCollection.BuildServiceProvider();
                }

                return _provider;
            }
        }

        public ServiceCollection ServiceCollection { get; set; } = new ServiceCollection();

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddMediatR(typeof(MediatRInstaller));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
            services.AddTransient<IRequestHandler<ExceptionHandlerFixtures.Command, Result>, ExceptionHandlerFixtures.GenericHandler>();
        }
    }
}