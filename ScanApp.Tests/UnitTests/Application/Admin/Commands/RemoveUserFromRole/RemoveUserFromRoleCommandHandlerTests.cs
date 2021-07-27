using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Commands.RemoveUserFromRole;
using ScanApp.Application.Common.Interfaces;
using System;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.RemoveUserFromRole
{
    public class RemoveUserFromRoleCommandHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new RemoveUserFromRoleCommandHandler(Mock.Of<IUserManager>(), Mock.Of<IRoleManager>(), Mock.Of<IContextFactory>());

            subject.Should().BeOfType<RemoveUserFromRoleCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IUserManager()
        {
            Action act = () => _ = new RemoveUserFromRoleCommandHandler(null, Mock.Of<IRoleManager>(), Mock.Of<IContextFactory>());

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IRoleManager()
        {
            Action act = () => _ = new RemoveUserFromRoleCommandHandler(Mock.Of<IUserManager>(), null, Mock.Of<IContextFactory>());

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IContextFactory()
        {
            Action act = () => _ = new RemoveUserFromRoleCommandHandler(Mock.Of<IUserManager>(), null, Mock.Of<IContextFactory>());

            act.Should().Throw<ArgumentNullException>();
        }
    }
}