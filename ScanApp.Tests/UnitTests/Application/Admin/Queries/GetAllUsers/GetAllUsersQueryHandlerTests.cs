using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Queries.GetAllUserRoles;
using ScanApp.Application.Admin.Queries.GetAllUsers;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandlerTests : IContextFactoryMockFixtures
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new GetAllUsersQueryHandler(ContextFactoryMock.Object);

            subject.Should().BeOfType<GetAllUsersQueryHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IContextFactory()
        {
            Action act = () => _ = new GetAllUsersQueryHandler(null);

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

        [Fact]
        public async Task Throws_if_exception_was_thrown_when_receiving_data()
        {
            ContextFactoryMock.Setup(c => c.CreateDbContext()).Throws<ArgumentNullException>();

            var subject = new GetAllUsersQueryHandler(ContextFactoryMock.Object);
            Func<Task> act = async () => await subject.Handle(new GetAllUsersQuery(), CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}