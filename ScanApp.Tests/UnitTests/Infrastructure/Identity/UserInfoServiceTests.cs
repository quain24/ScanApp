using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
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
        public async Task  GetUserConcurrencyStamp_returns_null_if_name_is_not_found()
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
        public async Task  GetUserVersion_returns_empty_version_if_name_is_not_found()
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
    }
}