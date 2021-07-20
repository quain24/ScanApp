using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using ScanApp.Application.Admin.Commands.DeleteUser;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.DeleteUser
{
    public class DeleteUserCommandHandlerTests : SqlLiteInMemoryDbFixture
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

        [Fact]
        public async Task Deletes_admin_user_if_there_is_another_admin_left()
        {
            var userManagerMock = new Mock<IUserManager>();
            userManagerMock.Setup(x =>
                x.DeleteUser(It.IsAny<string>())).ReturnsAsync(new Result());
            var factoryMock = new Mock<IContextFactory>();
            factoryMock.Setup(x => x.CreateDbContext()).Returns(NewDbContext);

            var roleManagerMock = new Mock<IRoleManager>();
            roleManagerMock.Setup(x => x.UsersInRole(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<string>()
            {
                UserGeneratorFixture.CreateValidListOfUsers()[0].UserName,
                UserGeneratorFixture.CreateValidListOfUsers()[1].UserName,
            });

            await using (var ctx = NewDbContext)
            {
                ctx.Users.AddRange(UserGeneratorFixture.CreateValidListOfUsers());
                ctx.Roles.Add(new IdentityRole(Globals.RoleNames.Administrator));
                await ctx.SaveChangesAsync();
                ctx.UserRoles.Add(new IdentityUserRole<string>()
                {
                    RoleId = ctx.Roles.First().Id,
                    UserId = ctx.Users.First().Id
                });
                await ctx.SaveChangesAsync();
                ctx.UserRoles.Add(new IdentityUserRole<string>()
                {
                    RoleId = ctx.Roles.First().Id,
                    UserId = UserGeneratorFixture.CreateValidListOfUsers()[0].Id
                });
                await ctx.SaveChangesAsync();
            }

            var command = new DeleteUserCommand(Globals.AccountNames.Administrator);
            var subject = new DeleteUserCommandHandler(userManagerMock.Object, roleManagerMock.Object, factoryMock.Object);

            var result = await subject.Handle(command, CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            userManagerMock.Verify(m => m.DeleteUser(It.Is<string>(x => x == Globals.AccountNames.Administrator)), Times.Once);
            userManagerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Will_not_delete_last_admin()
        {
            var userManagerMock = new Mock<IUserManager>();
            userManagerMock.Setup(x =>
                x.DeleteUser(It.IsAny<string>())).ReturnsAsync(new Result());
            userManagerMock.Setup(x => x.DeleteUser(It.IsAny<string>())).ReturnsAsync(new Result());
            var factoryMock = new Mock<IContextFactory>();
            factoryMock.Setup(x => x.CreateDbContext()).Returns(NewDbContext);

            var roleManagerMock = new Mock<IRoleManager>();
            // Returns empty list, because users are checked after deletion, so no users in role = there was only one, so cancel.
            roleManagerMock.Setup(x => x.UsersInRole(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<string>());

            await using (var ctx = NewDbContext)
            {
                ctx.Users.AddRange(UserGeneratorFixture.CreateValidListOfUsers());
                ctx.Roles.Add(new IdentityRole(Globals.RoleNames.Administrator));
                await ctx.SaveChangesAsync();
                ctx.UserRoles.Add(new IdentityUserRole<string>()
                {
                    RoleId = ctx.Roles.First().Id,
                    UserId = UserGeneratorFixture.CreateValidListOfUsers()[0].Id
                });
                await ctx.SaveChangesAsync();
            }

            var command = new DeleteUserCommand(Globals.AccountNames.Administrator);
            var subject = new DeleteUserCommandHandler(userManagerMock.Object, roleManagerMock.Object, factoryMock.Object);

            var result = await subject.Handle(command, CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.IllegalAccountOperation);
            userManagerMock.Verify(m => m.DeleteUser(It.Is<string>(x => x == Globals.AccountNames.Administrator)), Times.Once);
            userManagerMock.VerifyNoOtherCalls();
        }
    }
}