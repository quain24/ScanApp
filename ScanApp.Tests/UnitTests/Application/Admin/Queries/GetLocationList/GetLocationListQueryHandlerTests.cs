using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Queries.GetLocationList;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Queries.GetLocationList
{
    public class GetLocationListQueryHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var locationManagerMock = new Mock<ILocationManager>();
            var subject = new GetLocationListQueryHandler(locationManagerMock.Object);

            subject.Should().BeOfType<GetLocationListQueryHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IRoleManager()
        {
            Action act = () => _ = new GetLocationListQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Calls_GetAllLocations_ILocationManager_function()
        {
            var locationManagerMock = new Mock<ILocationManager>();
            var command = new GetLocationListQuery();
            var subject = new GetLocationListQueryHandler(locationManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            locationManagerMock.Verify(m => m.GetAllLocations(), Times.Once);
            locationManagerMock.VerifyNoOtherCalls();
        }
    }
}