using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ScanApp.Application.Common.Behaviors;
using ScanApp.Application.Common.Installers;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace ScanApp.Tests.IntegrationTests.Application
{
    public abstract class DbFixtureWithExtensionHandlers : SqlLiteInMemoryDbFixture
    {
        protected ITestOutputHelper Output { get; init; }

        protected override void ConfigureServices(ServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddMediatR(typeof(MediatRInstaller));
            services.AddSingleton<IHttpContextAccessor>(Mock.Of<IHttpContextAccessor>());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
            services.AddLogging(c => c.AddSerilog(new LoggerConfiguration()
                .WriteTo.TestOutput(Output)
                .Enrich.FromLogContext()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                .CreateLogger()));
        }
    }
}