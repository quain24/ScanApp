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
            var subject = new EmailValidator<string, string>();

            var context = new ValidationContext<string>("context");
            var result = subject.IsValid(context, email);

            result.Should().BeTrue();
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
            var subject = new EmailValidator<string, string>();

            var context = new ValidationContext<string>("context");
            var result = subject.IsValid(context, email);

            result.Should().BeFalse();
        }

        [Fact]
        public void Returns_false_if_email_is_null()
        {
            var subject = new EmailValidator<string, string>();

            var context = new ValidationContext<string>("context");
            var result = subject.IsValid(context, null);

            result.Should().BeFalse();
        }

        [Fact]
        public void Returns_false_if_email_is_not_a_string()
        {
            var subject = new EmailValidator<string, int>();

            var context = new ValidationContext<string>("context");
            var result = subject.IsValid(context, 1);

            result.Should().BeFalse();
        }
    }
}