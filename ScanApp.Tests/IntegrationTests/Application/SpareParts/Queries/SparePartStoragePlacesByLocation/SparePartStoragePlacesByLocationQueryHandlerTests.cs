using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.SpareParts.Queries.SparePartStoragePlacesByLocation;
using ScanApp.Tests.UnitTests.Application;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.IntegrationTests.Application.SpareParts.Queries.SparePartStoragePlacesByLocation
{
    public class SparePartStoragePlacesByLocationQueryHandlerTests : DbFixtureWithExtensionHandlers
    {
        public SparePartStoragePlacesByLocationQueryHandlerTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public async Task DatabaseIsAvailableAndCanBeConnectedTo()
        {
            Assert.True(await NewDbContext.Database.CanConnectAsync());
        }

        [Theory]
        [InlineData(typeof(OperationCanceledException))]
        [InlineData(typeof(TaskCanceledException))]
        public async Task Returns_invalid_result_of_cancelled_on_cancellation_or_timeout(Type type)
        {
            dynamic exc = Activator.CreateInstance(type);
            var contextFactoryMock = new IContextFactoryMockFixtures().ContextFactoryMock;
            contextFactoryMock.Setup(m => m.CreateDbContext()).Throws(exc);

            ServiceCollection.Replace(ServiceDescriptor.Singleton<IContextFactory>(contextFactoryMock.Object));
            var result = await Provider.GetService<IMediator>().Send(new SparePartStoragePlacesByLocationQuery("location_id"), CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Cancelled);
            result.ErrorDescription.Exception.Should().BeOfType(type);
        }
    }
}