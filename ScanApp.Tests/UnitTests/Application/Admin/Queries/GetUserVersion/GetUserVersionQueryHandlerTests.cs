using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Queries.GetUserVersion;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.Admin.Queries.GetUserVersion
{
    public class GetUserVersionQueryHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var userInfoMock = new Mock<IUserInfo>();
            var subject = new GetUserVersionQueryHandler(userInfoMock.Object);

            subject.Should().BeOfType<GetUserVersionQueryHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IUserInfo()
        {
            Action act = () => _ = new GetUserVersionQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Returns_valid_result()
        {
            var userInfoMock = new Mock<IUserInfo>();
            var stamp = Guid.NewGuid().ToString();
            userInfoMock.Setup(m => m.GetUserConcurrencyStamp(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(stamp);
            var subject = new GetUserVersionQueryHandler(userInfoMock.Object);

            var result = await subject.Handle(new GetUserVersionQuery("name"), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().Be(Version.Create(stamp));
        }

        [Fact]
        public async Task Returns_invalid_result_of_not_found_if_version_is_missing()
        {
            var userInfoMock = new Mock<IUserInfo>();
            userInfoMock.Setup(m => m.GetUserConcurrencyStamp(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as string);
            var subject = new GetUserVersionQueryHandler(userInfoMock.Object);

            var result = await subject.Handle(new GetUserVersionQuery("name"), CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            result.Output.Should().Be(Version.Empty);
        }

        [Fact]
        public async Task Throws_if_exception_was_thrown_when_receiving_data()
        {
            var userInfoMock = new Mock<IUserInfo>();
            userInfoMock.Setup(m => m.GetUserConcurrencyStamp("user_name", It.IsAny<CancellationToken>())).Throws<Exception>();

            var subject = new GetUserVersionQueryHandler(userInfoMock.Object);
            Func<Task> act = async () => await subject.Handle(new GetUserVersionQuery("user_name"), CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
        }
    }
}