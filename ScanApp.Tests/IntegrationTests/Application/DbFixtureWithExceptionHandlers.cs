using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ScanApp.Application.Common.Behaviors;
using ScanApp.Application.Common.Installers;
using Xunit.Abstractions;

namespace ScanApp.Tests.IntegrationTests.Application
{
    public abstract class DbFixtureWithExceptionHandlers : SqlLiteInMemoryDbFixture
    {
        protected ITestOutputHelper Output { get; init; }

        protected override void ConfigureServices(ServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddMediatR(typeof(MediatRInstaller));
            services.AddSingleton<IHttpContextAccessor>(Mock.Of<IHttpContextAccessor>());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
        }
    }
}