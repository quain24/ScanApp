using FluentAssertions;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Domain.ValueObjects;
using ScanApp.Infrastructure.Identity;
using Xunit;

namespace ScanApp.Tests.UnitTests.Infrastructure.Identity
{
    public class ResultHelpersTests
    {
        [Fact]
        public void UserNotFound_returns_invalid_result_of_not_found()
        {
            var result = ResultHelpers.UserNotFound("user_name");

            result.Should().BeOfType<Result>();
            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            result.ErrorDescription.ErrorMessage.Should().Contain("user_name");
        }

        [Fact]
        public void UserNotFound_generic_returns_invalid_result_of_given_type_with_code_not_found()
        {
            var result = ResultHelpers.UserNotFound<int>("user_name");

            result.Should().BeOfType<Result<int>>();
            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            result.ErrorDescription.ErrorMessage.Should().Contain("user_name");
        }

        [Fact]
        public void ConcurrencyError_returns_invalid_result_of_concurrency_error()
        {
            var result = ResultHelpers.ConcurrencyError(Version.Create("abc"), "test message");

            result.Should().BeOfType<Result<Version>>();
            result.Conclusion.Should().BeFalse();
            result.Output.Should().Be(Version.Create("abc"));
            result.ErrorDescription.ErrorMessage.Should().BeEquivalentTo("test message");
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ConcurrencyFailure);
        }

        [Fact]
        public void ConcurrencyError_given_no_message_will_return_empty_string_as_message()
        {
            var result = ResultHelpers.ConcurrencyError(Version.Create("abc"));

            result.Should().BeOfType<Result<Version>>();
            result.Conclusion.Should().BeFalse();
            result.Output.Should().Be(Version.Create("abc"));
            result.ErrorDescription.ErrorMessage.Should().BeEquivalentTo(string.Empty);
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ConcurrencyFailure);
        }

        [Fact]
        public void ConcurrencyError_given_no_version_will_return_empty_version_as_output()
        {
            var result = ResultHelpers.ConcurrencyError(null);

            result.Should().BeOfType<Result<Version>>();
            result.Conclusion.Should().BeFalse();
            result.Output.Should().Be(Version.Empty());
            result.ErrorDescription.ErrorMessage.Should().BeEquivalentTo(string.Empty);
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ConcurrencyFailure);
        }
    }
}