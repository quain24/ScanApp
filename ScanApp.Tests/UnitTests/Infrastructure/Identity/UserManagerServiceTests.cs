using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Infrastructure.Identity;
using ScanApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Infrastructure.Identity
{
    public class UserManagerServiceTests
    {
        [Fact]
        public void Will_create_user_manager()
        {
            var contextFactoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();

            var subject = new UserManagerService(UserManagerFixture.MockUserManager<ApplicationUser>(null).Object, contextFactoryMock.Object);

            subject.Should().NotBeNull().And.BeAssignableTo(typeof(IUserManager));
        }

        [Fact]
        public void Throws_arg_null_exc_if_no_UserManager_is_provided()
        {
            var contextFactoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();

            Action act = () => new UserManagerService(null, contextFactoryMock.Object);

            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().BeEquivalentTo(nameof(UserManager<ApplicationUser>));
        }

        [Fact]
        public void Throws_arg_null_exc_if_no_context_factory_is_provided()
        {
            Action act = () => new UserManagerService(UserManagerFixture.MockUserManager<ApplicationUser>(null).Object, null);

            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().BeEquivalentTo("ctxFactory");
        }

        [Fact]
        public async Task Will_add_user()
        {
            var locations = new List<Location>()
            {
                new("1", "location A"),
                new("2", "location B")
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var ctx = new ApplicationDbContext(options))
            {
                ctx.Locations.AddRange(locations);
                ctx.SaveChanges();
            }

            var ctxFacMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            ctxFacMock.Setup(c => c.CreateDbContext()).Returns(new ApplicationDbContext(options));

            var appusers = new List<ApplicationUser>();
            var userMgrMock = UserManagerFixture.MockUserManager(appusers);
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.AddNewUser("Adam", "password", "hw@wp.pl", "123456789", new Location("1", "location a"));

            result.Conclusion.Should().BeTrue("proper user data were supplied");
            result.Output.Name.Should().BeEquivalentTo("Adam");
            result.Output.Version.IsEmpty.Should().BeFalse("it is generated when new user is created");
            appusers.First().Email.Should().BeEquivalentTo("hw@wp.pl");
            appusers.First().PhoneNumber.Should().BeEquivalentTo("123456789");

            using (var ctx = new ApplicationDbContext(options))
            {
                ctx.UserLocations.Count().Should().Be(1);
                ctx.UserLocations.First().LocationId.Should().BeEquivalentTo("1");
                ctx.UserLocations.First().UserId.Should().BeEquivalentTo(appusers.First().Id);
            }
        }

        [Fact]
        public async Task Will_add_user_without_location()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var ctxFacMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            ctxFacMock.Setup(c => c.CreateDbContext()).Returns(new ApplicationDbContext(options));

            var appusers = new List<ApplicationUser>();
            var userMgrMock = UserManagerFixture.MockUserManager(appusers);
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.AddNewUser("Adam", "password", "hw@wp.pl", "123456789");

            result.Conclusion.Should().BeTrue("proper user data were supplied");
            result.Output.Name.Should().BeEquivalentTo("Adam");
            result.Output.Version.IsEmpty.Should().BeFalse("it is generated when new user is created");
            appusers.First().Email.Should().BeEquivalentTo("hw@wp.pl");
            appusers.First().PhoneNumber.Should().BeEquivalentTo("123456789");

            using (var ctx = new ApplicationDbContext(options))
            {
                ctx.UserLocations.Any().Should().BeFalse("no location was given");
            }
        }

        [Fact]
        public async Task Returns_failed_result_when_transaction_timeouts_when_adding_user()
        {
            var ctxFacMock = CreateSimpleFactoryMock();

            var appusers = new List<ApplicationUser>();
            var userMgrMock = UserManagerFixture.MockUserManager(appusers);
            userMgrMock.Setup(c => c.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ThrowsAsync(new TransactionAbortedException());
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.AddNewUser("Adam", "password", "hw@wp.pl", "123456789");

            result.Conclusion.Should().BeFalse("add always throws");
            result.Output.Should().BeNull();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Timeout);
        }

        [Fact]
        public async Task Return_failed_result_if_given_unknown_location()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var ctxFacMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            ctxFacMock.Setup(c => c.CreateDbContext()).Returns(new ApplicationDbContext(options));

            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.AddNewUser("Adam", "password", "hw@wp.pl", "123456789", new("2", "location B"));

            result.Conclusion.Should().BeFalse("unknown location has been supplied");
            result.Output.Should().BeNull();
            result.ErrorDescription.ErrorType.Should().BeEquivalentTo(ErrorType.NotFound);

            using (var ctx = new ApplicationDbContext(options))
            {
                ctx.UserLocations.Any().Should().BeFalse();
            }
        }

        [Fact]
        public async Task Return_failed_result_if_could_not_add_user()
        {
            var ctxFacMock = CreateSimpleFactoryMock();
            var appusers = new List<ApplicationUser>();
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, null, IdentityResult.Failed());
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.AddNewUser("Adam", "password", "hw@wp.pl", "123456789");

            result.Conclusion.Should().BeFalse("internal user manager always returns invalid result");
        }

        [Fact]
        public async Task Deletes_user_if_existing()
        {
            var ctxFacMock = CreateSimpleFactoryMock();
            var appusers = new List<ApplicationUser>();
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, findByNameResult: new ApplicationUser());
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.DeleteUser("name");

            result.Conclusion.Should().BeTrue();
            result.ResultType.Should().Be(ResultType.Deleted);
        }

        [Fact]
        public async Task Returns_not_found_result_if_tried_to_delete_non_existing_user()
        {
            var ctxFacMock = CreateSimpleFactoryMock();
            var appusers = new List<ApplicationUser>();
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, findByNameResult: null);
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.DeleteUser("name");

            result.Conclusion.Should().BeFalse("internal user manager will return null, meaning no such user exists");
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task ChangePassword_will_change_password()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var ctxFacMock = CreateSimpleFactoryMock();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            userMgrMock.Setup(m =>
                    m.ResetPasswordAsync(It.Is<ApplicationUser>(u => u.Id == user.Id), It.IsAny<string>(), It.Is<string>(s => s == "new password")))
                .ReturnsAsync(IdentityResult.Success);
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.ChangePassword(user.UserName, "new password", Version.Create(user.ConcurrencyStamp));

            result.Conclusion.Should().BeTrue();
        }

        [Fact]
        public async Task ChangePassword_returns_not_found_result_if_user_not_found()
        {
            var ctxFacMock = CreateSimpleFactoryMock();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.ChangePassword("wont.find", "new password", Version.Create("aaa"));

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            result.Output.Should().Be(Version.Empty());
        }

        [Fact]
        public async Task ChangePassword_will_return_concurrency_error_result_if_version_wont_match()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            var ctxFacMock = CreateSimpleFactoryMock();
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.ChangePassword(user.UserName, "new password", Version.Create("invalid"));

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ConcurrencyFailure);
            result.Output.Should().Be(Version.Create(user.ConcurrencyStamp), "failed password change because of concurrency will return current version");
        }

        [Fact]
        public async Task ValidatePassword_wont_return_anything_if_password_is_valid()
        {
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var passwordValidatorMock = new Mock<IPasswordValidator<ApplicationUser>>();
            var ctxFacMock = CreateSimpleFactoryMock();
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.ValidatePassword("P@ssword1");

            result.Should().BeEmpty("no errors were detected, all validators are satisfied");
        }

        [Fact]
        public async Task ValidatePassword_will_return_list_of_errors()
        {
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var passwordValidatorMock = new Mock<IPasswordValidator<ApplicationUser>>();
            var ctxFacMock = CreateSimpleFactoryMock();
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.ValidatePassword("Password1");

            result.Should().HaveCount(1, "one requirement is not satisfied (special char)")
                .And
                .Subject.First().Code.Should().Be("PasswordRequiresNonAlphanumeric");
        }

        [Fact]
        public async Task ValidatePassword_will_throw_arg_null_if_given_null()
        {
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var ctxFacMock = CreateSimpleFactoryMock();
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            Func<Task> act = async () => await sut.ValidatePassword(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task EditUserData_will_change_data()
        {
            var user = UserGeneratorFixture.CreateValidUser();

            var locations = new List<Location>
            {
                new("1", "location A"),
                new("2", "location B")
            };

            var editDto = new EditUserDto(user.UserName)
            {
                Location = locations[1],
                Email = "new_mail@o2.pl",
                Version = Version.Create(user.ConcurrencyStamp),
                NewName = "new_user_name",
                Phone = "321654987"
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var ctx = new ApplicationDbContext(options))
            {
                ctx.Locations.AddRange(locations);
                ctx.Users.Add(user);
                ctx.UserLocations.Add(new UserLocation { LocationId = "1", UserId = user.Id });
                ctx.SaveChanges();
            }

            var ctxFacMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            ctxFacMock.Setup(c => c.CreateDbContext()).Returns(new ApplicationDbContext(options));

            var userManagerMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success).Callback(() =>
            {
                using var ctx = new ApplicationDbContext(options);
                ctx.Users.Update(user);
                ctx.SaveChanges();
            });

            var sut = new UserManagerService(userManagerMock.Object, ctxFacMock.Object);
            var result = await sut.EditUserData(editDto);

            result.Conclusion.Should().BeTrue();

            using (var ctx = new ApplicationDbContext(options))
            {
                ctx.UserLocations.Should().HaveCount(1);

                var location = ctx.UserLocations.First();
                location.LocationId.Should().Be(locations[1].Id);
                location.UserId.Should().Be(user.Id);

                ctx.Users.Should().HaveCount(1);
                ctx.Users.First().Email.Should().Be(editDto.Email);
                ctx.Users.First().UserName.Should().Be(editDto.NewName);
                ctx.Users.First().ConcurrencyStamp.Should().Be(user.ConcurrencyStamp);
                ctx.Users.First().PhoneNumber.Should().Be(editDto.Phone);
            }
        }

        [Fact]
        public async Task EditUserData_will_change_data_without_location()
        {
            var user = UserGeneratorFixture.CreateValidUser();

            var locations = new List<Location>
            {
                new("1", "location A"),
                new("2", "location B")
            };

            var editDto = new EditUserDto(user.UserName)
            {
                Email = "new_mail@o2.pl",
                Version = Version.Create(user.ConcurrencyStamp),
                NewName = "new_user_name",
                Phone = "321654987"
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var ctx = new ApplicationDbContext(options))
            {
                ctx.Locations.AddRange(locations);
                ctx.Users.Add(user);
                ctx.UserLocations.Add(new UserLocation { LocationId = "1", UserId = user.Id });
                ctx.SaveChanges();
            }

            var ctxFacMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            ctxFacMock.Setup(c => c.CreateDbContext()).Returns(new ApplicationDbContext(options));

            var userManagerMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success).Callback(() =>
            {
                using var ctx = new ApplicationDbContext(options);
                ctx.Users.Update(user);
                ctx.SaveChanges();
            });

            var sut = new UserManagerService(userManagerMock.Object, ctxFacMock.Object);
            var result = await sut.EditUserData(editDto);

            result.Conclusion.Should().BeTrue();

            using (var ctx = new ApplicationDbContext(options))
            {
                ctx.UserLocations.Should().HaveCount(1);

                var location = ctx.UserLocations.First();
                location.LocationId.Should().Be(locations[0].Id);
                location.UserId.Should().Be(user.Id);

                ctx.Users.Should().HaveCount(1);
                ctx.Users.First().Email.Should().Be(editDto.Email);
                ctx.Users.First().UserName.Should().Be(editDto.NewName);
                ctx.Users.First().ConcurrencyStamp.Should().Be(user.ConcurrencyStamp);
                ctx.Users.First().PhoneNumber.Should().Be(editDto.Phone);
            }
        }

        [Fact]
        public async Task EditUserData_will_return_not_found_result_if_there_is_no_user_with_given_name()
        {
            var editDto = new EditUserDto("unknown_name") { Version = Version.Create("random") };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var ctxFacMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            ctxFacMock.Setup(c => c.CreateDbContext()).Returns(new ApplicationDbContext(options));

            var userManagerMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));

            var sut = new UserManagerService(userManagerMock.Object, ctxFacMock.Object);
            var result = await sut.EditUserData(editDto);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
        }

        public static IEnumerable<object[]> GetInvalidVersions()
        {
            yield return new object[] { Version.Empty() };
            yield return new object[] { Version.Create("not_match") };
        }

        [Theory]
        [MemberData(nameof(GetInvalidVersions))]
        public async Task EditUserData_will_return_concurrency_error_result_if_there_is_no_version_or_mismatch_version(Version version)
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var editDto = new EditUserDto("unknown_name") { Version = version };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var ctxFacMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            ctxFacMock.Setup(c => c.CreateDbContext()).Returns(new ApplicationDbContext(options));

            var userManagerMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);

            var sut = new UserManagerService(userManagerMock.Object, ctxFacMock.Object);
            var result = await sut.EditUserData(editDto);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ConcurrencyFailure);
        }

        private static Mock<IDbContextFactory<ApplicationDbContext>> CreateSimpleFactoryMock()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var ctxFacMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            ctxFacMock.Setup(c => c.CreateDbContext()).Returns(new ApplicationDbContext(options));
            return ctxFacMock;
        }
    }
}