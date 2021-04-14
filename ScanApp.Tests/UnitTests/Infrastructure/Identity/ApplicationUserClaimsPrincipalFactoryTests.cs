using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using ScanApp.Application.Common.Entities;
using ScanApp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Infrastructure.Identity
{
    public class ApplicationUserClaimsPrincipalFactoryTests
    {
        public ITestOutputHelper Output { get; }

        public ApplicationUserClaimsPrincipalFactoryTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Will_create_instance()
        {
            var userManagerMock = UserManagerFixture.MockUserManager<ApplicationUser>();
            var roleManagerMock = RoleManagerFixture.MockRoleManager();
            var optionsAccessorMock = new Mock<IOptions<IdentityOptions>>();
            var optionsMock = new Mock<IdentityOptions>();
            optionsAccessorMock.Setup(o => o.Value).Returns(optionsMock.Object);
            var dbContextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();

            var subject = new ApplicationUserClaimsPrincipalFactory(userManagerMock.Object, roleManagerMock.Object, optionsAccessorMock.Object, dbContextFactoryMock.Object);

            subject.Should().NotBeNull()
                .And.BeOfType<ApplicationUserClaimsPrincipalFactory>()
                .And.Subject.Should().BeAssignableTo<UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>>();
        }

        [Fact]
        public void Will_throw_arg_null_exc_if_instantiated_with_null_ctx_factory()
        {
            var userManagerMock = UserManagerFixture.MockUserManager<ApplicationUser>();
            var roleManagerMock = RoleManagerFixture.MockRoleManager();
            var optionsAccessorMock = new Mock<IOptions<IdentityOptions>>();
            var optionsMock = new Mock<IdentityOptions>();
            optionsAccessorMock.Setup(o => o.Value).Returns(optionsMock.Object);

            Action act = () => new ApplicationUserClaimsPrincipalFactory(userManagerMock.Object, roleManagerMock.Object, optionsAccessorMock.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Will_create_claims_principal_from_given_user()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = CreateUserManagerMock(user);
            userManagerMock.Setup(u => u.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<Claim>(0));
            var roleManagerMock = RoleManagerFixture.MockRoleManager();
            var optionsAccessorMock = new Mock<IOptions<IdentityOptions>>();
            var options = new IdentityOptions();
            optionsAccessorMock.Setup(o => o.Value).Returns(options);
            var dbContextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
            var sut = new ApplicationUserClaimsPrincipalFactory(userManagerMock.Object, roleManagerMock.Object, optionsAccessorMock.Object, dbContextFactoryMock.Object);

            var result = await sut.CreateAsync(user);

            result.Identity.Name.Should().Be(user.UserName);
            result.FindFirst(options.ClaimsIdentity.UserIdClaimType).Value.Should().Be(user.Id);
            result.FindFirst(options.ClaimsIdentity.EmailClaimType).Value.Should().Be(user.Email);
            result.FindFirst(options.ClaimsIdentity.SecurityStampClaimType).Value.Should().Be(user.SecurityStamp);

            foreach (var claim in result.Claims)
            {
                Output.WriteLine(claim.Type);
                Output.WriteLine(claim.Value);
            }
        }

        [Fact]
        public async Task Returned_claim_principal_will_not_contain_duplicates()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = CreateUserManagerMock(user);
            var claims = new List<Claim>
            {
                new("Type_a", "value_a"),
                new("Type_a", "value_a"),
                new("Type_a", "value_a"),
                new("Type_b", "value_b"),
                new("Type_b", "value_b"),
                new("Type_c", "value_c"),
            };
            userManagerMock.Setup(u => u.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(claims);
            var roleManagerMock = RoleManagerFixture.MockRoleManager();
            var optionsAccessorMock = new Mock<IOptions<IdentityOptions>>();
            var options = new IdentityOptions();
            optionsAccessorMock.Setup(o => o.Value).Returns(options);
            var dbContextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
            var sut = new ApplicationUserClaimsPrincipalFactory(userManagerMock.Object, roleManagerMock.Object, optionsAccessorMock.Object, dbContextFactoryMock.Object);

            var result = await sut.CreateAsync(user);

            result.Identity.Name.Should().Be(user.UserName);
            result.FindFirst(options.ClaimsIdentity.UserIdClaimType).Value.Should().Be(user.Id);
            result.FindFirst(options.ClaimsIdentity.EmailClaimType).Value.Should().Be(user.Email);
            result.FindFirst(options.ClaimsIdentity.SecurityStampClaimType).Value.Should().Be(user.SecurityStamp);
            result.Claims.Should().OnlyHaveUniqueItems(c => c.Type)
                .And.OnlyHaveUniqueItems(c => c.Value);

            foreach (var claim in result.Claims)
            {
                Output.WriteLine(claim.Type);
                Output.WriteLine(claim.Value);
            }
        }

        private Mock<UserManager<ApplicationUser>> CreateUserManagerMock(ApplicationUser user)
        {
            var mock = UserManagerFixture.MockUserManager<ApplicationUser>();
            mock.Setup(u => u.GetUserIdAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(user.Id);
            mock.Setup(u => u.GetUserNameAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(user.UserName);
            mock.Setup(u => u.GetEmailAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(user.Email);
            mock.Setup(u => u.GetPhoneNumberAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(user.PhoneNumber);
            mock.Setup(u => u.GetSecurityStampAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(user.SecurityStamp);
            mock.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>(0));

            return mock;
        }
    }
}