using FluentAssertions;
using ScanApp.Application.Common.Helpers.Result;
using System;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Common.Helpers.Result
{
    public class ErrorDescriptionTests
    {
        [Fact]
        public void Creates_instance_with_random_guid()
        {
            var subject = new ErrorDescription();
            var compare = new ErrorDescription();

            subject.Guid.Should().NotBeEmpty()
                .And.Subject.Value.As<string>().Should().NotBeEquivalentTo(compare.Guid.ToString());
        }

        [Fact]
        public void ToString_returns_given_code_and_message_if_both_present()
        {
            var subject = new ErrorDescription
            {
                ErrorType = ErrorType.Canceled,
                ErrorMessage = "message"
            };

            subject.ToString().Should().Be($"{subject.ErrorType} - {subject.ErrorMessage}");
        }

        [Fact]
        public void ToString_returns_given_code_if_only_code_is_present()
        {
            var subject = new ErrorDescription
            {
                ErrorType = ErrorType.Canceled
            };

            subject.ToString().Should().Be(subject.ErrorType.ToString());
        }

        [Fact]
        public void StackTrace_is_set_if_exception_is_given()
        {
            Exception exception;
            try
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            var subject = new ErrorDescription { Exception = exception };

            subject.StackTrace.Should().Be(exception.StackTrace);
        }
    }
}