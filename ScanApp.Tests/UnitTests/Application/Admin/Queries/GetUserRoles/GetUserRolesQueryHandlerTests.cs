using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using ScanApp.Application.Admin;
using ScanApp.Application.Admin.Queries.GetUserRoles;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.Admin.Queries.GetUserRoles
{
    public class GetUserRolesQueryHandlerTests : IContextFactoryMockFixtures
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new GetUserRolesQueryHandler(ContextFactoryMock.Object);

            subject.Should().BeOfType<GetUserRolesQueryHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IContextFactory()
        {
            Action act = () => _ = new GetUserRolesQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Retrieves_data()
        {
            var users = UserGeneratorFixture.CreateValidListOfUsers();
            var roles = new List<IdentityRole>
            {
                new() {ConcurrencyStamp = Guid.NewGuid().ToString(), Id = "r1", Name = "role_name_1", NormalizedName = "ROLE_NAME_1"},
                new() {ConcurrencyStamp = Guid.NewGuid().ToString(), Id = "r2", Name = "role_name_2", NormalizedName = "ROLE_NAME_2"}
            };
            var userRoles = new List<IdentityUserRole<string>>
            {
                new() {RoleId = "r1", UserId = users[0].Id},
                new() {RoleId = "r2", UserId = users[1].Id}
            };
            var usersMock = users.AsQueryable().BuildMockDbSet();
            var rolesMock = roles.AsQueryable().BuildMockDbSet();
            var userRolesMock = userRoles.AsQueryable().BuildMockDbSet();
            ContextMock.Setup(m => m.Users).Returns(usersMock.Object);
            ContextMock.Setup(m => m.Roles).Returns(rolesMock.Object);
            ContextMock.Setup(m => m.UserRoles).Returns(userRolesMock.Object);

            var subject = new GetUserRolesQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new GetUserRolesQuery(users[0].UserName, Version.Create(users[0].ConcurrencyStamp)), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().HaveCount(1);
            result.Output.First().Should().BeEquivalentTo(new BasicRoleModel(roles[0].Name, Version.Create(roles[0].ConcurrencyStamp)));
        }

        [Fact]
        public async Task Returns_empty_result_if_user_has_no_roles_assigned()
        {
            var users = UserGeneratorFixture.CreateValidListOfUsers();
            var roles = new List<IdentityRole>
            {
                new() {ConcurrencyStamp = Guid.NewGuid().ToString(), Id = "r1", Name = "role_name_1", NormalizedName = "ROLE_NAME_1"},
                new() {ConcurrencyStamp = Guid.NewGuid().ToString(), Id = "r2", Name = "role_name_2", NormalizedName = "ROLE_NAME_2"}
            };
            var userRoles = new List<IdentityUserRole<string>>
            {
                new() {RoleId = "r1", UserId = users[1].Id},
                new() {RoleId = "r2", UserId = users[1].Id}
            };
            var usersMock = users.AsQueryable().BuildMockDbSet();
            var rolesMock = roles.AsQueryable().BuildMockDbSet();
            var userRolesMock = userRoles.AsQueryable().BuildMockDbSet();
            ContextMock.Setup(m => m.Users).Returns(usersMock.Object);
            ContextMock.Setup(m => m.Roles).Returns(rolesMock.Object);
            ContextMock.Setup(m => m.UserRoles).Returns(userRolesMock.Object);

            var subject = new GetUserRolesQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new GetUserRolesQuery(users[0].UserName, Version.Create(users[0].ConcurrencyStamp)), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEmpty();
        }

        [Fact]
        public async Task Returns_invalid_result_of_not_found_if_user_does_not_exist()
        {
            var usersMock = new List<ApplicationUser>(0).AsQueryable().BuildMockDbSet();
            ContextMock.Setup(m => m.Users).Returns(usersMock.Object);
            var subject = new GetUserRolesQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new GetUserRolesQuery("name", Version.Create("version")), CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task Returns_invalid_result_of_concurrency_failure_if_given_version_does_not_match_db_version()
        {
            var users = UserGeneratorFixture.CreateValidListOfUsers();
            var usersMock = users.AsQueryable().BuildMockDbSet();
            ContextMock.Setup(m => m.Users).Returns(usersMock.Object);
            var subject = new GetUserRolesQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new GetUserRolesQuery(users[0].UserName, Version.Create("mismatch_version")), CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ConcurrencyFailure);
        }

        [Fact]
        public async Task Throws_if_exception_was_thrown_when_receiving_data()
        {
            ContextFactoryMock.Setup(c => c.CreateDbContext()).Throws<ArgumentNullException>();

            var subject = new GetUserRolesQueryHandler(ContextFactoryMock.Object);
            Func<Task> act = async () => await subject.Handle(new GetUserRolesQuery("user_name", Version.Create("version")), CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(OperationCanceledException))]
        [InlineData(typeof(TaskCanceledException))]
        public async Task Returns_invalid_result_of_cancelled_on_cancellation_or_timeout(Type type)
        {
            dynamic exc = Activator.CreateInstance(type);
            ContextFactoryMock.Setup(m => m.CreateDbContext()).Throws(exc);

            var subject = new GetUserRolesQueryHandler(ContextFactoryMock.Object);
            var result = await subject.Handle(new GetUserRolesQuery("user_name", Version.Create("version")), CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Canceled);
            result.ErrorDescription.Exception.Should().BeOfType(type);
        }
    }
}