using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Queries.GetAllClaims;
using ScanApp.Application.Admin.Queries.GetAllClaimsFromRole;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Queries.GetAllClaimsFromRole
{
    public class GetAllClaimsFromRoleQueryHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var roleManagerMock = new Mock<IRoleManager>();
            var subject = new GetAllClaimsFromRoleQueryHandler(roleManagerMock.Object);

            subject.Should().BeOfType<GetAllClaimsFromRoleQueryHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IContextFactory()
        {
            Action act = () => _ = new GetAllClaimsQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Calls_GetAllClaimsFromRole_IRoleManager_function()
        {
            var roleManagerMock = new Mock<IRoleManager>();
            var command = new GetAllClaimsFromRoleQuery("role_name");
            var subject = new GetAllClaimsFromRoleQueryHandler(roleManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            roleManagerMock.Verify(m => m.GetAllClaimsFromRole("role_name"), Times.Once);
            roleManagerMock.VerifyNoOtherCalls();
        }
    }
}