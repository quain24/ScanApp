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
using System.Reflection;
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
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();

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
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);

            using (var ctx = new ApplicationDbContext(options))
            {
                ctx.UserLocations.Any().Should().BeFalse();
            }
        }

        [Fact]
        public async Task Return_failed_result_if_could_not_add_user()
        {
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
            var appusers = new List<ApplicationUser>();
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, null, IdentityResult.Failed());
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.AddNewUser("Adam", "password", "hw@wp.pl", "123456789");

            result.Conclusion.Should().BeFalse("internal user manager always returns invalid result");
        }

        [Fact]
        public async Task Deletes_user_if_existing()
        {
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
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
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
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
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
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
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.ChangePassword("wont.find", "new password", Version.Create("none"));

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            result.Output.Should().Be(Version.Empty);
        }

        [Fact]
        public async Task ChangePassword_will_return_concurrency_error_result_if_version_wont_match()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
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
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.ValidatePassword("P@ssword1");

            result.Should().BeEmpty("no errors were detected, all validators are satisfied");
        }

        [Fact]
        public async Task ValidatePassword_will_return_list_of_errors()
        {
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
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
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            Func<Task> act = async () => await sut.ValidatePassword(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
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
            yield return new object[] { Version.Empty };
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

        [Fact]
        public async Task EditUserData_will_return_invalid_result_if_internal_user_update_fails()
        {
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
            var appusers = new List<ApplicationUser>();
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, findByNameResult: user);
            userMgrMock.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Failed(new[] { new IdentityError() { Code = "testCode", Description = "test description" } }));
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.EditUserData(new EditUserDto(user.UserName) { Version = Version.Create(user.ConcurrencyStamp) });

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotValid);
            result.ErrorDescription.ErrorMessage.Should().Be("testCode | test description");
        }

        [Fact]
        public async Task EditUserData_will_return_concurrency_error_result_if_database_operation_fails_with_dbconcurrency_exc()
        {
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
            var appusers = new List<ApplicationUser>();
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, findByNameResult: user);
            userMgrMock.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>())).ThrowsAsync(new DbUpdateConcurrencyException());
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.EditUserData(new EditUserDto(user.UserName) { Version = Version.Create(user.ConcurrencyStamp) });

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ConcurrencyFailure);
            result.ErrorDescription.ErrorMessage.Should().Be("User or location has been changed during this command.");
            result.ErrorDescription.Exception.Should().NotBeNull().And.BeAssignableTo<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditUserData_will_return_unknown_error_result_if_database_operation_fails_with_dbupdate_exc()
        {
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
            var appusers = new List<ApplicationUser>();
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, findByNameResult: user);
            userMgrMock.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>())).ThrowsAsync(new DbUpdateException());
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.EditUserData(new EditUserDto(user.UserName) { Version = Version.Create(user.ConcurrencyStamp) });

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Unknown);
            result.ErrorDescription.ErrorMessage.Should().Be($"Something happened during update of {user.UserName}.");
            result.ErrorDescription.Exception.Should().NotBeNull().And.BeAssignableTo<DbUpdateException>();
        }

        [Fact]
        public async Task EditUserData_will_return_timeout_error_result_if_database_operation_fails_transaction_aborted_exc()
        {
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
            var appusers = new List<ApplicationUser>();
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, findByNameResult: user);
            userMgrMock.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>())).ThrowsAsync(new TransactionAbortedException());
            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.EditUserData(new EditUserDto(user.UserName) { Version = Version.Create(user.ConcurrencyStamp) });

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Timeout);
            result.ErrorDescription.ErrorMessage.Should().Be("User or location has been changed during this command.");
            result.ErrorDescription.Exception.Should().NotBeNull().And.BeAssignableTo<TransactionAbortedException>();
        }

        public static IEnumerable<object[]> GetPartialUpdateDtos()
        {
            yield return new object[] { new EditUserDto(UserGeneratorFixture.CreateValidUser().UserName)
            {
                Version = Version.Create(UserGeneratorFixture.CreateValidUser().ConcurrencyStamp),
                Email = "new_email@wp.pl"
            }};
            yield return new object[] { new EditUserDto(UserGeneratorFixture.CreateValidUser().UserName)
            {
                Version = Version.Create(UserGeneratorFixture.CreateValidUser().ConcurrencyStamp),
                NewName = "new_name"
            }};
            yield return new object[] { new EditUserDto(UserGeneratorFixture.CreateValidUser().UserName)
            {
                Version = Version.Create(UserGeneratorFixture.CreateValidUser().ConcurrencyStamp),
                Phone = "123555555",
                NewName = "a_new_name"
            }};
        }

        [Theory]
        [MemberData(nameof(GetPartialUpdateDtos))]
        public async Task EditUserData_will_update_only_provided_data(EditUserDto editData)
        {
            var databaseId = Guid.NewGuid().ToString();
            var ctxFacMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(databaseId);
            var comparedUser = UserGeneratorFixture.CreateValidUser();
            var user = UserGeneratorFixture.CreateValidUser();
            var appusers = new List<ApplicationUser> { user };
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, findByNameResult: user);

            using (var ctx = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: databaseId).Options))
            {
                ctx.Add(user);
                ctx.SaveChanges();
            }

            comparedUser.Id = user.Id;
            comparedUser.ConcurrencyStamp = user.ConcurrencyStamp;
            comparedUser.SecurityStamp = user.SecurityStamp;
            comparedUser.PasswordHash = user.PasswordHash;

            userMgrMock.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success).Callback(() =>
            {
                using var ctx = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: databaseId).Options);
                ctx.Update(user);
                ctx.SaveChanges();
            });

            var sut = new UserManagerService(userMgrMock.Object, ctxFacMock.Object);

            var result = await sut.EditUserData(editData);
            var props = typeof(EditUserDto).GetProperties();
            var userProps = typeof(ApplicationUser).GetProperties();

            foreach (var up in userProps)
            {
                PropertyInfo dtoProp;
                if (up.Name == nameof(user.UserName))
                    dtoProp = props.FirstOrDefault(p => p.Name == nameof(editData.NewName));
                else if (up.Name == nameof(user.PhoneNumber))
                    dtoProp = props.FirstOrDefault(p => p.Name == nameof(editData.Phone));
                else
                    dtoProp = props.FirstOrDefault(p => p.Name == up.Name);

                var orgProp = userProps.FirstOrDefault(p => p.Name == up.Name);
                if (dtoProp?.GetValue(editData) is not null)
                {
                    dtoProp.GetValue(editData).Should().BeEquivalentTo(up.GetValue(user), $"dto have {dtoProp?.Name} value, so it should replace user {up?.Name} value.");
                }
                else
                {
                    up.GetValue(user).Should().BeEquivalentTo(orgProp.GetValue(comparedUser), $"dto does not have {dtoProp?.Name ?? "unknown name"} value, so  user {up?.Name} value should match compared user {orgProp?.Name ?? "unknown name"}");
                }
            }

            result.Conclusion.Should().BeTrue();
            result.Output.Should().Be(Version.Create(user.ConcurrencyStamp));
        }

        [Fact]
        public async Task HasLocation_will_return_valid_result_of_true_if_user_has_location()
        {
            var dbId = Guid.NewGuid().ToString();
            var user = UserGeneratorFixture.CreateValidUser();
            var location = new Location("1", "location_name");
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                ctx.Add(user);
                ctx.Add(location);
                ctx.SaveChanges();
                ctx.Add(new UserLocation { LocationId = location.Id, UserId = user.Id });
                ctx.SaveChanges();
            }

            var appusers = new List<ApplicationUser> { user };
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, findByNameResult: user);

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object);
            var result = await sut.HasLocation(user.UserName);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeTrue();
        }

        [Fact]
        public async Task HasLocation_will_return_valid_result_of_false_if_user_has_no_location()
        {
            var dbId = Guid.NewGuid().ToString();
            var user = UserGeneratorFixture.CreateValidUser();
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                ctx.Add(user);
                ctx.SaveChanges();
            }

            var appusers = new List<ApplicationUser> { user };
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, findByNameResult: user);

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object);
            var result = await sut.HasLocation(user.UserName);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeFalse();
        }

        [Fact]
        public async Task HasLocation_will_return_invalid_result_if_no_user_is_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);
            var result = await sut.HasLocation(user.UserName);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            result.Output.Should().BeFalse();
        }

        [Fact]
        public async Task GetUserLocation_will_return_location_when_user_has_one()
        {
            var dbId = Guid.NewGuid().ToString();
            var user = UserGeneratorFixture.CreateValidUser();
            var location = new Location("1", "location_name");
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                ctx.Add(user);
                ctx.Add(location);
                ctx.SaveChanges();
                ctx.Add(new UserLocation() { LocationId = location.Id, UserId = user.Id });
                ctx.SaveChanges();
            }

            var appusers = new List<ApplicationUser> { user };
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, findByNameResult: user);

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object);
            var result = await sut.GetUserLocation(user.UserName);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeOfType<Location>();
            result.Output.Id.Should().Be("1");
            result.Output.Name.Should().Be("location_name");
        }

        [Fact]
        public async Task GetUserLocation_will_return_valid_result_with_null_when_user_has_no_location()
        {
            var dbId = Guid.NewGuid().ToString();
            var user = UserGeneratorFixture.CreateValidUser();
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                ctx.Add(user);
                ctx.Add(new UserLocation() { LocationId = "location_id", UserId = "other_id" });
                ctx.SaveChanges();
            }

            var appusers = new List<ApplicationUser> { user };
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, findByNameResult: user);

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object);
            var result = await sut.GetUserLocation(user.UserName);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeNull();
        }

        [Fact]
        public async Task GetUserLocation_will_return_invalid_result_not_found_when_user_has_is_not_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();

            var appusers = new List<ApplicationUser> { user };
            var userMgrMock = UserManagerFixture.MockUserManager(appusers);

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);
            var result = await sut.GetUserLocation(user.UserName);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task SetUserLocation_will_set_new_location_for_user()
        {
            var dbId = Guid.NewGuid().ToString();
            var user = UserGeneratorFixture.CreateValidUser();
            var location = new Location("1", "location_name");
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                ctx.Add(user);
                ctx.Add(location);
                ctx.SaveChanges();
            }

            var appusers = new List<ApplicationUser> { user };
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, findByNameResult: user);
            userMgrMock.Setup(u => u.GenerateConcurrencyStampAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(Guid.NewGuid().ToString());

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object);

            var result = await sut.SetUserLocation(user.UserName, location, Version.Create(user.ConcurrencyStamp));

            result.Conclusion.Should().BeTrue();
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                var cstamp = ctx.Users.First().ConcurrencyStamp;
                result.Output.Should().Be(Version.Create(cstamp));
            }
        }

        [Fact]
        public async Task SetUserLocation_will_replace_old_location_with_new()
        {
            var dbId = Guid.NewGuid().ToString();
            var user = UserGeneratorFixture.CreateValidUser();
            var oldLocation = new Location("1", "location_name");
            var newLocation = new Location("2", "new_location_name");
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                ctx.Add(user);
                ctx.Add(oldLocation);
                ctx.Add(newLocation);
                ctx.Add(new UserLocation() { LocationId = oldLocation.Id, UserId = user.Id });
                ctx.SaveChanges();
            }

            var appusers = new List<ApplicationUser> { user };
            var userMgrMock = UserManagerFixture.MockUserManager(appusers, findByNameResult: user);
            userMgrMock.Setup(u => u.GenerateConcurrencyStampAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(Guid.NewGuid().ToString());

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object);

            var result = await sut.SetUserLocation(user.UserName, newLocation, Version.Create(user.ConcurrencyStamp));

            result.Conclusion.Should().BeTrue();
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                var cstamp = ctx.Users.First().ConcurrencyStamp;
                result.Output.Should().Be(Version.Create(cstamp));
                ctx.UserLocations.Should().HaveCount(1);
                var newLocationData = ctx.UserLocations.First();
                newLocationData.UserId.Should().Be(user.Id);
                newLocationData.LocationId.Should().Be(newLocation.Id);
            }
        }

        [Fact]
        public async Task SetUserLocation_will_throw_arg_null_exc_when_null_given_instead_location()
        {
            var factoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));

            var sut = new UserManagerService(userMgrMock.Object, factoryMock.Object);
            Func<Task> act = async () => await sut.SetUserLocation("random_name", null, Version.Create("random stamp"));

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task SetUserLocation_will_return_not_found_invalid_result_if_there_is_no_user_with_given_name()
        {
            var factoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));

            var sut = new UserManagerService(userMgrMock.Object, factoryMock.Object);

            var result = await sut.SetUserLocation("unknown", new Location("not important"), Version.Create("test"));

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task SetUserLocation_will_return_concurrency_error_invalid_result_if_given_version_is_not_valid()
        {
            var dbId = Guid.NewGuid().ToString();
            var factoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId);
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(), findByNameResult: user);

            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                ctx.Add(user);
                ctx.SaveChanges();
            }

            var sut = new UserManagerService(userMgrMock.Object, factoryMock.Object);

            var result = await sut.SetUserLocation(user.UserName, new Location("not important"), Version.Create("test"));

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ConcurrencyFailure);
        }

        [Fact]
        public async Task SetUserLocation_will_return_invalid_result_with_unknown_error_if_dbupdate_exc_was_thrown()
        {
            var dbId = Guid.NewGuid().ToString();
            var factoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId);
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(), findByNameResult: user);
            userMgrMock.Setup(u => u.GenerateConcurrencyStampAsync(It.IsAny<ApplicationUser>())).ThrowsAsync(new DbUpdateException());

            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                ctx.Add(user);
                ctx.SaveChanges();
            }

            var sut = new UserManagerService(userMgrMock.Object, factoryMock.Object);

            var result = await sut.SetUserLocation(user.UserName, new Location("not important"), Version.Create(user.ConcurrencyStamp));
            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Unknown);
        }

        [Fact]
        public async Task RemoveFromLocation_will_remove_user_from_location_when_he_have_one()
        {
            var dbId = Guid.NewGuid().ToString();
            var user = UserGeneratorFixture.CreateValidUser();
            var location = new Location("1", "location_name");
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                ctx.Add(user);
                ctx.Add(location);
                ctx.Add(new UserLocation() { LocationId = location.Id, UserId = user.Id });
                ctx.SaveChanges();
            }

            var appusers = new List<ApplicationUser> { user };
            var userMgrMock = UserManagerFixture.MockUserManager(appusers);
            userMgrMock.Setup(u => u.GenerateConcurrencyStampAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(Guid.NewGuid().ToString());

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object);

            var result = await sut.RemoveFromLocation(user.UserName, Version.Create(user.ConcurrencyStamp));

            result.Conclusion.Should().BeTrue();
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                ctx.UserLocations.Any(u => u.UserId == user.Id).Should().BeFalse();
                var cstamp = ctx.Users.First(u => u.Id == user.Id).ConcurrencyStamp;
                result.Output.Should().Be(Version.Create(cstamp));
            }
        }

        [Fact]
        public async Task RemoveFromLocation_will_return_concurrency_error_bad_result_if_version_mismatch()
        {
            var dbId = Guid.NewGuid().ToString();
            var user = UserGeneratorFixture.CreateValidUser();
            var location = new Location("1", "location_name");
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                ctx.Add(user);
                ctx.Add(location);
                ctx.Add(new UserLocation() { LocationId = location.Id, UserId = user.Id });
                ctx.SaveChanges();
            }

            var appusers = new List<ApplicationUser> { user };
            var userMgrMock = UserManagerFixture.MockUserManager(appusers);
            userMgrMock.Setup(u => u.GenerateConcurrencyStampAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(Guid.NewGuid().ToString());

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object);

            var result = await sut.RemoveFromLocation(user.UserName, Version.Create("invalid"));

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ConcurrencyFailure);
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(dbId).Object.CreateDbContext())
            {
                ctx.UserLocations.Any(u => u.UserId == user.Id).Should().BeTrue();
                result.Output.Should().Be(Version.Create(user.ConcurrencyStamp));
            }
        }

        [Fact]
        public async Task RemoveFromLocation_returns_not_found_bad_result_if_no_user_with_given_name_exists()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.RemoveFromLocation(user.UserName, Version.Create(user.ConcurrencyStamp));

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task ChangeUserSecurityStamp_will_change_security_stamp()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            userMgrMock.Setup(u => u.UpdateSecurityStampAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.ChangeUserSecurityStamp(user.UserName, Version.Create(user.ConcurrencyStamp));

            result.Conclusion.Should().BeTrue();
            userMgrMock.Verify(u => u.UpdateSecurityStampAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [Fact]
        public async Task ChangeUserSecurityStamp_returns_invalid_result_of_concurrency_error_if_version_mismatch()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            userMgrMock.Setup(u => u.UpdateSecurityStampAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.ChangeUserSecurityStamp(user.UserName, Version.Create("invalid"));

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ConcurrencyFailure);
            userMgrMock.Verify(u => u.UpdateSecurityStampAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }

        [Fact]
        public async Task ChangeUserSecurityStamp_returns_invalid_result_of_not_found_if_there_is_no_user_with_given_name()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.ChangeUserSecurityStamp(user.UserName, Version.Create(user.ConcurrencyStamp));

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            userMgrMock.Verify(u => u.UpdateSecurityStampAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }

        [Fact]
        public async Task AddUserToRole_will_add_proper_user_to_role()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var role = "role_name";
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            userMgrMock.Setup(u => u.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.Is<string[]>(r => r.First() == role))).ReturnsAsync(IdentityResult.Success);
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.AddUserToRole(user.UserName, Version.Create(user.ConcurrencyStamp), role);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().NotBeNull();
            userMgrMock.Verify(u => u.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<string[]>()), Times.Once);
        }

        [Fact]
        public async Task AddUserToRole_will_add_proper_user_to_multiple_roles()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var roles = new[] { "role_name", "new_role", "additional_role" };
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            userMgrMock.Setup(u => u.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.Is<string[]>(r => r.SequenceEqual(roles)))).ReturnsAsync(IdentityResult.Success);
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.AddUserToRole(user.UserName, Version.Create(user.ConcurrencyStamp), roles);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().NotBeNull();
            userMgrMock.Verify(u => u.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<string[]>()), Times.Once);
        }

        [Fact]
        public async Task AddUserToRole_will_return_bad_result_of_concurrency_error_on_version_mismatch()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var role = "role_name";
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.AddUserToRole(user.UserName, Version.Create("random"), role);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ConcurrencyFailure);
            result.Output.Should().Be(Version.Create(user.ConcurrencyStamp), "on failure - current version is returned");
            userMgrMock.Verify(u => u.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<string[]>()), Times.Never);
        }

        [Fact]
        public async Task AddUserToRole_will_return_bad_result_of_not_found_if_user_is_not_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var role = "role_name";
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.AddUserToRole(user.UserName, Version.Create(user.ConcurrencyStamp), role);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            result.Output.Should().Be(Version.Empty, "on failure - empty version is returned, since no user is found to create one from");
            userMgrMock.Verify(u => u.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<string[]>()), Times.Never);
        }

        [Theory]
        [InlineData("a_role")]
        [InlineData("a_role", "another_role")]
        public async Task RemoveUserFromRole_will_remove_given_roles_from_given_user(params string[] roles)
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            userMgrMock.Setup(u => u.RemoveFromRolesAsync(It.Is<ApplicationUser>(u => u.UserName.Equals(user.UserName)),
                It.Is<string[]>(m => m.SequenceEqual(roles)))).ReturnsAsync(IdentityResult.Success);
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.RemoveUserFromRole(user.UserName, Version.Create(user.ConcurrencyStamp), roles);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeOfType<Version>().And.Should().NotBe(Version.Empty);
            userMgrMock.Verify(u => u.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<string[]>()), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromRole_will_return_invalid_result_if_user_is_not_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.RemoveUserFromRole(user.UserName, Version.Create(user.ConcurrencyStamp), "a_role");

            result.Conclusion.Should().BeFalse();
            result.Output.Should().Be(Version.Empty);
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            userMgrMock.Verify(u => u.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<string[]>()), Times.Never);
        }

        [Fact]
        public async Task RemoveUserFromRole_will_return_invalid_result_if_version_mismatch()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.RemoveUserFromRole(user.UserName, Version.Create("invalid"), "a_role");

            result.Conclusion.Should().BeFalse();
            result.Output.Should().Be(Version.Create(user.ConcurrencyStamp));
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ConcurrencyFailure);
            userMgrMock.Verify(u => u.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<string[]>()), Times.Never);
        }

        [Fact]
        public async Task IsInRole_will_return_valid_result_of_true_if_user_is_in_role()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            userMgrMock.Setup(u => u.IsInRoleAsync(It.Is<ApplicationUser>(u => u.Id == user.Id), It.Is<string>(u => u.Equals("role_name")))).ReturnsAsync(true);
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.IsInRole(user.UserName, "role_name");

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeTrue();
            userMgrMock.Verify(u => u.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task IsInRole_will_return_invalid_result_of_false_if_user_is_not_fount()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.IsInRole(user.UserName, "role_name");

            result.Conclusion.Should().BeFalse();
            result.Output.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            userMgrMock.Verify(u => u.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AddClaimToUser_will_add_claim_to_given_user()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var claim = new Claim("type", "value");
            var claimList = new List<System.Security.Claims.Claim>(1);
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);

            userMgrMock.Setup(u => u.GetClaimsAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<System.Security.Claims.Claim>());
            userMgrMock.Setup(u => u.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<ApplicationUser, System.Security.Claims.Claim>((_, c) => claimList.Add(c));

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.AddClaimToUser(user.UserName, claim.Type, claim.Value);

            result.Conclusion.Should().BeTrue();
            claimList.Should().HaveCount(1);
            claimList.First().Type.Should().Be("type");
            claimList.First().Value.Should().Be("value");
        }

        [Fact]
        public async Task AddClaimToUser_will_return_bad_result_of_duplicated_if_user_already_have_given_claim()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var claim = new Claim("type", "value");
            var claimList = new List<System.Security.Claims.Claim>(1) { new(claim.Type, claim.Value) };
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);

            userMgrMock.Setup(u => u.GetClaimsAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(claimList);

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.AddClaimToUser(user.UserName, claim.Type, claim.Value);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Duplicated);
            claimList.Should().HaveCount(1);
            claimList.First().Type.Should().Be("type");
            claimList.First().Value.Should().Be("value");
        }

        [Theory]
        [InlineData("", "value")]
        [InlineData("type", "")]
        [InlineData("type", null)]
        [InlineData(null, "value")]
        [InlineData(null, null)]
        [InlineData("", "")]
        public async Task AddClaimToUser_will_return_bad_result_of_not_valid_if_given_claim_is_not_valid(string type, string value)
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);

            userMgrMock.Setup(u => u.GetClaimsAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<System.Security.Claims.Claim>(0));

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.AddClaimToUser(user.UserName, type, value);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotValid);
            userMgrMock.Verify(u => u.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>()), Times.Never);
        }

        [Fact]
        public async Task AddClaimToUser_will_return_bad_result_of_not_found_if_there_is_no_user_with_given_name()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var claim = new Claim("type", "value");
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));

            userMgrMock.Setup(u => u.GetClaimsAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<System.Security.Claims.Claim>(0));

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.AddClaimToUser(user.UserName, claim.Type, claim.Value);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task RemoveClaimFromUser_removes_existing_claim()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var claim = new Claim("type", "value");
            var claimList = new List<System.Security.Claims.Claim>(1) { new(claim.Type, claim.Value) };
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);

            userMgrMock.Setup(u => u.GetClaimsAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(claimList);
            userMgrMock.Setup(u => u.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<ApplicationUser, System.Security.Claims.Claim>((_, c) => claimList.Remove(c));

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.RemoveClaimFromUser(user.UserName, claim.Type, claim.Value);

            result.Conclusion.Should().BeTrue();
            claimList.Should().BeEmpty();
            userMgrMock.Verify(u => u.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>()), Times.Once);
        }

        [Fact]
        public async Task RemoveClaimFromUser_returns_valid_result_if_there_was_no_such_claim_to_delete()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var claim = new Claim("type", "value");
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);

            userMgrMock.Setup(u => u.GetClaimsAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<System.Security.Claims.Claim>(0));

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.RemoveClaimFromUser(user.UserName, claim.Type, claim.Value);

            result.Conclusion.Should().BeTrue();
            userMgrMock.Verify(u => u.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>()), Times.Never);
        }

        [Fact]
        public async Task RemoveClaimFromUser_returns_invalid_result_of_not_found_if_user_was_not_found()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var claim = new Claim("type", "value");
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.RemoveClaimFromUser(user.UserName, claim.Type, claim.Value);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            userMgrMock.Verify(u => u.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>()), Times.Never);
        }

        [Theory]
        [InlineData("", "value")]
        [InlineData("type", "")]
        [InlineData("type", null)]
        [InlineData(null, "value")]
        [InlineData(null, null)]
        [InlineData("", "")]
        public async Task RemoveClaimFromUser_will_return_valid_result_if_given_invalid_claim_to_delete(string type, string value)
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);

            userMgrMock.Setup(u => u.GetClaimsAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<System.Security.Claims.Claim>(0));

            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.RemoveClaimFromUser(user.UserName, type, value);

            result.Conclusion.Should().BeTrue();
            userMgrMock.Verify(u => u.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>()), Times.Never);
        }

        [Fact]
        public async Task SetLockoutDate_will_set_lockout_date()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0), findByNameResult: user);
            userMgrMock.Setup(u => u.SetLockoutEndDateAsync(It.IsAny<ApplicationUser>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(IdentityResult.Success);

            var date = new DateTimeOffset(1999, 1, 1, 12, 0, 0, TimeSpan.Zero);
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.SetLockoutDate(user.UserName, date);

            result.Conclusion.Should().BeTrue();
            userMgrMock.Verify(u => u.SetLockoutEndDateAsync(It.IsAny<ApplicationUser>(), It.Is<DateTimeOffset>(d => d.Equals(date))), Times.Once);
        }

        [Fact]
        public async Task SetLockoutDate_will_return_invalid_result_of_not_found_if_there_is_no_user_with_given_name()
        {
            var user = UserGeneratorFixture.CreateValidUser();
            var userMgrMock = UserManagerFixture.MockUserManager(new List<ApplicationUser>(0));

            var date = new DateTimeOffset(1999, 1, 1, 12, 0, 0, TimeSpan.Zero);
            var sut = new UserManagerService(userMgrMock.Object, AppDbContextFactoryMockFixture.CreateSimpleFactoryMock().Object);

            var result = await sut.SetLockoutDate(user.UserName, date);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            userMgrMock.Verify(u => u.SetLockoutEndDateAsync(It.IsAny<ApplicationUser>(), It.Is<DateTimeOffset>(d => d.Equals(date))), Times.Never);
        }
    }
}