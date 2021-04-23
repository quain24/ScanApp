using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Queries.GetAllUserData;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Queries.GetAllUserData
{
    public class GetAllUserDataQueryHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var userInfoMock = new Mock<IUserInfo>();
            var subject = new GetAllUserDataQueryHandler(userInfoMock.Object);

            subject.Should().BeOfType<GetAllUserDataQueryHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IUserInfo()
        {
            Action act = () => _ = new GetAllUserDataQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Calls_GetData_IUserInfo_function()
        {
            var userInfoMock = new Mock<IUserInfo>();
            var command = new GetAllUserDataQuery("user_name");
            var subject = new GetAllUserDataQueryHandler(userInfoMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            userInfoMock.Verify(m => m.GetData("user_name"), Times.Once);
            userInfoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Returns_valid_result_of_ok_if_user_is_found()
        {
            var userInfoMock = new Mock<IUserInfo>();
            userInfoMock.Setup(c => c.GetData(It.IsAny<string>())).ReturnsAsync(new UserInfoModel { Name = "user_name" });
            var command = new GetAllUserDataQuery("user_name");
            var subject = new GetAllUserDataQueryHandler(userInfoMock.Object);

            var result = await subject.Handle(command, CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeOfType<UserInfoModel>();
            result.Output.Name.Should().Be("user_name");
        }

        [Fact]
        public async Task Returns_invalid_result_of_not_found_if_user_does_not_exist()
        {
            var userInfoMock = new Mock<IUserInfo>();
            var command = new GetAllUserDataQuery("user_name");
            var subject = new GetAllUserDataQueryHandler(userInfoMock.Object);

            var result = await subject.Handle(command, CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
        }
    }
}