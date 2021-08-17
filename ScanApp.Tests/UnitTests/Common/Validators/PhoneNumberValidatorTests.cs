using FluentAssertions;
using ScanApp.Common.Validators;
using Xunit;

namespace ScanApp.Tests.UnitTests.Common.Validators
{
    public class PhoneNumberValidatorTests
    {
        [Theory]
        [InlineData("123456")]
        [InlineData("123456789")]
        [InlineData("0012347852587896214857896")]
        [InlineData("+012347852587896214857896")]
        public void Validates_proper_phone_number(string phone)
        {
            var subject = new PhoneNumberValidator();

            var result = subject.Validate(phone);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("123-456-789")]
        [InlineData("(00)12347852587")]
        [InlineData("+ 01234785258789621485")]
        [InlineData("+48 123 789 621 485")]
        [InlineData("123456D")]
        public void Invalidates_wrong_phone_number(string phone)
        {
            var subject = new PhoneNumberValidator();

            var result = subject.Validate(phone);

            result.IsValid.Should().BeFalse();
        }
    }
}