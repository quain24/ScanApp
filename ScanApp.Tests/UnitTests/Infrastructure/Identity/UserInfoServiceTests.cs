using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using ScanApp.Application.Admin.Queries.GetAllUserData;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Infrastructure.Identity
{
    public class UserInfoServiceTests
    {
        [Fact]
        public void Will_create_UserInfoService()
        {
            var userManagerMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            sut.Should().NotBeNull()
                .And
                .BeOfType<UserInfoService>()
                .And
                .BeAssignableTo<IUserInfo>();
        }

        [Fact]
        public void Will_throw_arg_null_if_provided_user_manager_was_null()
        {
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            Action Act = () => new UserInfoService(claimsFacMock.Object, null, userManagerServiceMock.Object);

            Act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Will_throw_arg_null_if_provided_claims_factory_was_null()
        {
            var userManagerMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var userManagerServiceMock = new Mock<IUserManager>();

            Action Act = () => new UserInfoService(null, userManagerMock.Object, userManagerServiceMock.Object);

            Act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Will_throw_arg_null_if_provided_user_manager_service_was_null()
        {
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));

            Action Act = () => new UserInfoService(claimsFacMock.Object, userManagerMock.Object, null);

            Act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task UserExists_returns_true_if_user_exists()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userList = new List<ApplicationUser> { user };
            var userManagerMock = UserManagerFixture.MockUserManager(userList);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.UserExists(user.UserName);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserExists_returns_false_if_no_user_with_given_name_exists()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userList = new List<ApplicationUser> { user };
            var userManagerMock = UserManagerFixture.MockUserManager(userList);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.UserExists("other_name");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetUserNameById_get_name_if_id_is_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userList = new List<ApplicationUser> { user };
            var userManagerMock = UserManagerFixture.MockUserManager(userList);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetUserNameById(user.Id);

            result.Should().Be(user.UserName);
        }

        [Fact]
        public async Task GetUserNameById_returns_null_if_id_is_not_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userList = new List<ApplicationUser> { user };
            var userManagerMock = UserManagerFixture.MockUserManager(userList);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetUserNameById("unknown_id");

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserIdByName_get_name_if_name_is_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userList = new List<ApplicationUser> { user };
            var userManagerMock = UserManagerFixture.MockUserManager(userList);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetUserIdByName(user.UserName);

            result.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetUserIdByName_returns_null_if_name_is_not_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userList = new List<ApplicationUser> { user };
            var userManagerMock = UserManagerFixture.MockUserManager(userList);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetUserIdByName("unknown_name");

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserConcurrencyStamp_get_concurrency_stamp_if_name_is_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userList = new List<ApplicationUser> { user };
            var userManagerMock = UserManagerFixture.MockUserManager(userList);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetUserConcurrencyStamp(user.UserName);

            result.Should().Be(user.ConcurrencyStamp);
        }

        [Fact]
        public async Task GetUserConcurrencyStamp_returns_null_if_name_is_not_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userList = new List<ApplicationUser> { user };
            var userManagerMock = UserManagerFixture.MockUserManager(userList);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetUserConcurrencyStamp("unknown_name");

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserVersion_returns_proper_version_if_name_is_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userList = new List<ApplicationUser> { user };
            var userManagerMock = UserManagerFixture.MockUserManager(userList);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetUserVersion(user.UserName);

            result.Should().Be(Version.Create(user.ConcurrencyStamp));
        }

        [Fact]
        public async Task GetUserVersion_returns_empty_version_if_name_is_not_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userList = new List<ApplicationUser> { user };
            var userManagerMock = UserManagerFixture.MockUserManager(userList);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetUserVersion("unknown_name");

            result.Should().Be(Version.Empty());
        }

        [Fact]
        public async Task GetData_returns_data_of_existing_user()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager(findByNameResult: user);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();
            userManagerServiceMock.Setup(u => u.GetUserLocation(user.UserName)).ReturnsAsync(new Result<Location>(new Location("1", "location_name")));

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetData(user.UserName);

            result.Should().NotBeNull().And.BeOfType<UserInfoModel>();
            result.Location.Id.Should().Be("1");
            result.Location.Name.Should().Be("location_name");
            result.Email.Should().Be(user.Email);
            result.LockoutEnd.Should().Be(user.LockoutEnd);
            result.Name.Should().Be(user.UserName);
            result.Phone.Should().Be(user.PhoneNumber);
            result.Version.Should().Be(Version.Create(user.ConcurrencyStamp));
        }

        [Fact]
        public async Task GetData_returns_null_if_no_user_was_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager<ApplicationUser>();
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetData(user.UserName);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllRoles_will_return_list_of_all_user_roles()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager(findByNameResult: user);
            var roles = new List<string> { "role_one", "role_two" };
            userManagerMock.Setup(u => u.GetRolesAsync(It.Is<ApplicationUser>(u => u.Id == user.Id))).ReturnsAsync(roles);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetAllRoles(user.UserName);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().HaveCount(roles.Count).And.BeEquivalentTo(roles);
        }

        [Fact]
        public async Task GetAllRoles_will_return_invalid_result_of_not_found_if_user_does_not_exist()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager<ApplicationUser>();
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetAllRoles(user.UserName);

            result.Conclusion.Should().BeFalse();
            userManagerMock.Verify(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()), Times.Never);
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task IsInRole_will_return_valid_result_of_true_if_user_is_in_role()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager(findByNameResult: user);
            var roles = new List<string> { "role_one", "role_two" };
            userManagerMock.Setup(u =>
                u.IsInRoleAsync(It.Is<ApplicationUser>(u => u.Id == user.Id), It.Is<string>(s => roles.Contains(s))))
                .ReturnsAsync(true);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.IsInRole(user.UserName, roles[0]);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeTrue();
        }

        [Fact]
        public async Task IsInRole_will_return_valid_result_of_false_if_user_is_not_in_role()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager(findByNameResult: user);
            var roles = new List<string> { "role_one", "role_two" };
            userManagerMock.Setup(u =>
                    u.IsInRoleAsync(It.Is<ApplicationUser>(u => u.Id == user.Id), It.Is<string>(s => roles.Contains(s))))
                .ReturnsAsync(true);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.IsInRole(user.UserName, "third_role");

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeFalse();
        }

        [Fact]
        public async Task IsInRole_will_return_invalid_result_of_false_if_user_is_not_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager<ApplicationUser>();
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.IsInRole(user.UserName, "any_role");

            result.Conclusion.Should().BeFalse();
            result.Output.Should().BeFalse();
            userManagerMock.Verify(u => u.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetAllClaims_will_return_list_of_all_user_claims()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager(findByNameResult: user);
            IList<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim> { new("type_one", "value_one"), new("type_2", "value_2") };
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.SetupGet(cp => cp.Claims).Returns(claims);
            claimsFacMock.Setup(cf => cf.CreateAsync(It.Is<ApplicationUser>(u => u.Id == user.Id)))
                .ReturnsAsync(claimsPrincipalMock.Object);
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetAllClaims(user.UserName);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().HaveCount(claims.Count);
            result.Output.Should().BeEquivalentTo(claims, options => options.ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetAllClaims_will_return_invalid_result_of_not_found_if_user_was_not_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager<ApplicationUser>();
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetAllClaims(user.UserName);

            result.Conclusion.Should().BeFalse();
            result.Output.Should().BeNull();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            claimsFacMock.Verify(c => c.CreateAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }

        [Theory]
        [InlineData("type_one")]
        [InlineData("type_one", "type_two")]
        public async Task GetClaims_will_return_list_of_user_claims_of_given_types(params string[] claimTypes)
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager(findByNameResult: user);
            IList<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>
            {
                new ("type_one", "value_one"),
                new ("type_two", "value_two"),
                new ("type_three", "value_three")
            };
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.SetupGet(cp => cp.Claims).Returns(claims);
            claimsFacMock.Setup(cf => cf.CreateAsync(It.Is<ApplicationUser>(u => u.Id == user.Id)))
                .ReturnsAsync(claimsPrincipalMock.Object);
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetClaims(user.UserName, claimTypes);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().HaveCount(claimTypes.Length);
            result.Output.Select(c => c.Value).Should().BeSubsetOf(claims.Select(c => c.Value));
        }

        [Fact]
        public async Task GetClaims_will_return_empty_list_of_user_claims_when_not_types_match()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager(findByNameResult: user);
            IList<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>
            {
                new ("type_one", "value_one"),
                new ("type_two", "value_two")
            };
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.SetupGet(cp => cp.Claims).Returns(claims);
            claimsFacMock.Setup(cf => cf.CreateAsync(It.Is<ApplicationUser>(u => u.Id == user.Id)))
                .ReturnsAsync(claimsPrincipalMock.Object);
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetClaims(user.UserName, "not_found_type");

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEmpty();
            claimsFacMock.Verify(c => c.CreateAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [Fact]
        public async Task GetClaims_will_return_invalid_result_of_not_found_if_user_was_not_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager<ApplicationUser>();
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.GetClaims(user.UserName, "not_important");

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            claimsFacMock.Verify(c => c.CreateAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }

        [Fact]
        public async Task HasClaim_returns_true_if_user_has_claim_of_given_type_and_value()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager(findByNameResult: user);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.Setup(cp => cp.HasClaim(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            claimsFacMock.Setup(cf => cf.CreateAsync(It.Is<ApplicationUser>(u => u.Id == user.Id)))
                .ReturnsAsync(claimsPrincipalMock.Object);
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.HasClaim(user.UserName, "test_type", "test_value");

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeTrue();
        }

        [Fact]
        public async Task HasClaim_returns_good_result_of_false_if_user_does_not_have_claim_of_given_type_and_value()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager(findByNameResult: user);
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.Setup(cp => cp.HasClaim(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            claimsFacMock.Setup(cf => cf.CreateAsync(It.Is<ApplicationUser>(u => u.Id == user.Id)))
                .ReturnsAsync(claimsPrincipalMock.Object);
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.HasClaim(user.UserName, "test_type", "test_value");

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeFalse();
        }

        [Fact]
        public async Task HasClaim_will_return_invalid_result_of_not_found_if_user_was_not_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userManagerMock = UserManagerFixture.MockUserManager<ApplicationUser>();
            var claimsFacMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerServiceMock = new Mock<IUserManager>();

            var sut = new UserInfoService(claimsFacMock.Object, userManagerMock.Object, userManagerServiceMock.Object);

            var result = await sut.HasClaim(user.UserName, "test_type", "test_value");

            result.Conclusion.Should().BeFalse();
            result.Output.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
        }
    }
}