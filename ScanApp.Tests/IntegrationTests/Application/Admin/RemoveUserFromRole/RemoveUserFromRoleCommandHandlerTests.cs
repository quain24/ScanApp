using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using ScanApp.Application.Admin.Commands.RemoveUserFromRole;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.IntegrationTests.Application.Admin.RemoveUserFromRole
{
    public class RemoveUserFromRoleCommandHandlerTests : SqlLiteInMemoryDbFixture
    {
        [Fact]
        public async Task Removes_user_if_one_other_has_admin_role()
        {
            var userManagerMock = new Mock<IUserManager>();
            userManagerMock.Setup(x =>
                x.RemoveUserFromRole(It.IsAny<string>(), It.IsAny<Version>(), It.IsAny<string>())).ReturnsAsync(new Result<Version>(Version.Create("OK")));
            var command = new RemoveUserFromRoleCommand("name", Version.Create("version"), Globals.RoleNames.Administrator);
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

            var subject = new RemoveUserFromRoleCommandHandler(userManagerMock.Object, roleManagerMock.Object, factoryMock.Object);

            var result = await subject.Handle(command, CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            userManagerMock.Verify(m => m.RemoveUserFromRole("name", Version.Create("version"), Globals.RoleNames.Administrator), Times.Once);
            userManagerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Will_not_remove_user_from_role_if_he_is_last_admin()
        {
            var userManagerMock = new Mock<IUserManager>();
            userManagerMock.Setup(x =>
                x.RemoveUserFromRole(It.IsAny<string>(), It.IsAny<Version>(), It.IsAny<string>())).ReturnsAsync(new Result<Version>(Version.Create("last")));
            var command = new RemoveUserFromRoleCommand("name", Version.Create("version"), Globals.RoleNames.Administrator);
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

            var subject = new RemoveUserFromRoleCommandHandler(userManagerMock.Object, roleManagerMock.Object, factoryMock.Object);

            var result = await subject.Handle(command, CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.IllegalAccountOperation);
            userManagerMock.Verify(m => m.RemoveUserFromRole("name", Version.Create("version"), Globals.RoleNames.Administrator), Times.Once);
            userManagerMock.VerifyNoOtherCalls();
        }
    }
}