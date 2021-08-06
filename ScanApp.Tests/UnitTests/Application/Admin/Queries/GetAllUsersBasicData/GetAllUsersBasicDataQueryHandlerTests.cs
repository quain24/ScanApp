using FluentAssertions;
using MediatR;
using MockQueryable.Moq;
using ScanApp.Application.Admin.Queries.GetAllUsersBasicData;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.Admin.Queries.GetAllUsersBasicData
{
    public class GetAllUsersBasicDataQueryHandlerTests : IContextFactoryMockFixtures
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new GetAllUsersBasicDataQueryHandler(ContextFactoryMock.Object);

            subject.Should().BeOfType<GetAllUsersBasicDataQueryHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IContextFactory()
        {
            Action act = () => _ = new GetAllUsersBasicDataQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Throws_if_exception_was_thrown_when_receiving_data()
        {
            ContextFactoryMock.Setup(c => c.CreateDbContext()).Throws<ArgumentNullException>();

            var subject = new GetAllUsersBasicDataQueryHandler(ContextFactoryMock.Object);
            Func<Task> act = async () => await subject.Handle(new GetAllUsersBasicDataQuery(), CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Returns_valid_result_with_data()
        {
            var users = UserGeneratorFixture.CreateValidListOfUsers();
            var usersMock = users.AsQueryable().BuildMockDbSet();
            ContextMock.Setup(m => m.Users).Returns(usersMock.Object);
            var subject = new GetAllUsersBasicDataQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new GetAllUsersBasicDataQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().Equal(users, (resultModel, originalUser) =>
                resultModel.Version.Equals(Version.Create(originalUser.ConcurrencyStamp)) &&
                resultModel.Name.Equals(originalUser.UserName));
        }

        [Fact]
        public async Task Returns_valid_empty_result_if_there_is_no_data()
        {
            var usersMock = new List<ApplicationUser>().AsQueryable().BuildMockDbSet();
            ContextMock.Setup(m => m.Users).Returns(usersMock.Object);
            var subject = new GetAllUsersBasicDataQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new GetAllUsersBasicDataQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEmpty();
        }
    }
}