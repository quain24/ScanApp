using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Commands.DeleteUser;
using ScanApp.Application.Common.Interfaces;
using System;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.DeleteUser
{
    public class DeleteUserCommandHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new DeleteUserCommandHandler(Mock.Of<IUserManager>(), Mock.Of<IRoleManager>(), Mock.Of<IContextFactory>());

            subject.Should().BeOfType<DeleteUserCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IRoleManager()
        {
            Action act = () => _ = new DeleteUserCommandHandler(Mock.Of<IUserManager>(), null, Mock.Of<IContextFactory>());

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IContextFactory()
        {
            Action act = () => _ = new DeleteUserCommandHandler(null, Mock.Of<IRoleManager>(), null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IUserManager()
        {
            Action act = () => _ = new DeleteUserCommandHandler(null, Mock.Of<IRoleManager>(), Mock.Of<IContextFactory>());

            act.Should().Throw<ArgumentNullException>();
        }
    }
}