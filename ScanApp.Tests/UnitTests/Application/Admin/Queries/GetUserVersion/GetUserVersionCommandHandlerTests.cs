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
    public class GetUserVersionCommandHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var userInfoMock = new Mock<IUserInfo>();
            var subject = new GetUserVersionCommandHandler(userInfoMock.Object);

            subject.Should().BeOfType<GetUserVersionCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IUserInfo()
        {
            Action act = () => _ = new GetUserVersionCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Returns_valid_result()
        {
            var userInfoMock = new Mock<IUserInfo>();
            var stamp = Guid.NewGuid().ToString();
            userInfoMock.Setup(m => m.GetUserConcurrencyStamp(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(stamp);
            var subject = new GetUserVersionCommandHandler(userInfoMock.Object);

            var result = await subject.Handle(new GetUserVersionCommand("name"), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().Be(Version.Create(stamp));
        }

        [Fact]
        public async Task Returns_invalid_result_of_not_found_if_version_is_missing()
        {
            var userInfoMock = new Mock<IUserInfo>();
            userInfoMock.Setup(m => m.GetUserConcurrencyStamp(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(null as string);
            var subject = new GetUserVersionCommandHandler(userInfoMock.Object);

            var result = await subject.Handle(new GetUserVersionCommand("name"), CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            result.Output.Should().Be(Version.Empty());
        }

        [Theory]
        [InlineData(typeof(OperationCanceledException))]
        [InlineData(typeof(TaskCanceledException))]
        public async Task Returns_invalid_result_of_cancelled_on_cancellation_or_timeout(Type type)
        {
            dynamic exc = Activator.CreateInstance(type);
            var userInfoMock = new Mock<IUserInfo>();
            userInfoMock.Setup(m => m.GetUserConcurrencyStamp("name", It.IsAny<CancellationToken>())).Throws(exc);

            var subject = new GetUserVersionCommandHandler(userInfoMock.Object);
            var result = await subject.Handle(new GetUserVersionCommand("name"), CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Cancelled);
            result.ErrorDescription.Exception.Should().BeOfType(type);
        }

        [Fact]
        public async Task Throws_if_exception_was_thrown_when_receiving_data()
        {
            var userInfoMock = new Mock<IUserInfo>();
            userInfoMock.Setup(m => m.GetUserConcurrencyStamp("user_name", It.IsAny<CancellationToken>())).Throws<Exception>();

            var subject = new GetUserVersionCommandHandler(userInfoMock.Object);
            Func<Task> act = async () => await subject.Handle(new GetUserVersionCommand("user_name"), CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
        }
    }
}