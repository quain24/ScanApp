using FluentAssertions;
using FluentValidation;
using ScanApp.Common.Validators;
using Xunit;

namespace ScanApp.Tests.UnitTests.Common.Validators
{
    public class EmailValidatorTests
    {
        [Theory]
        [InlineData("email@wp.pl")]
        [InlineData("email.wp@wp.pl")]
        [InlineData("email.new_and_long_address@wp.pl")]
        [InlineData("email.new.123@wp.pl")]
        [InlineData("email.new.123@wp.berlin")]
        public void Returns_true_for_valid_email(string email)
        {
            var subject = new EmailValidator();

            var result = subject.Validate(email);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("  email@wp.pl")]
        [InlineData("email..@wp.pl")]
        [InlineData("email.@wp.pl")]
        [InlineData("email.new.@wp.pl")]
        [InlineData("email.new.123@ wp.pl")]
        [InlineData("email.new.123@wp.berlinnnedg")]
        [InlineData("email.new.123@pllll")]
        [InlineData("email.new<.123@pllll.cop")]
        public void Returns_false_if_email_is_invalid(string email)
        {
            var subject = new EmailValidator();
            
            var result = subject.Validate(email);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Returns_false_if_email_is_null()
        {
            var subject = new EmailValidator();
            
            var result = subject.Validate(null as string);

            result.IsValid.Should().BeFalse();
        }
    }
}