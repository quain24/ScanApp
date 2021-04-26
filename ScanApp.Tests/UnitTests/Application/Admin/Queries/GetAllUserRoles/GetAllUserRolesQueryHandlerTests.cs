using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Queries.GetAllUserRoles;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Queries.GetAllUserRoles
{
    public class GetAllUserRolesQueryHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var roleManagerMock = new Mock<IRoleManager>();
            var subject = new GetAllUserRolesQueryHandler(roleManagerMock.Object);

            subject.Should().BeOfType<GetAllUserRolesQueryHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IContextFactory()
        {
            Action act = () => _ = new GetAllUserRolesQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Calls_GetAllRoles_IRoleManager_function()
        {
            var roleManagerMock = new Mock<IRoleManager>();
            var command = new GetAllUserRolesQuery();
            var subject = new GetAllUserRolesQueryHandler(roleManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            roleManagerMock.Verify(m => m.GetAllRoles(), Times.Once);
            roleManagerMock.VerifyNoOtherCalls();
        }
    }
}