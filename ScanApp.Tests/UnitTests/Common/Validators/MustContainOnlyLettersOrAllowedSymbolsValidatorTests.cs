using FluentAssertions;
using FluentValidation;
using ScanApp.Common.Validators;
using Xunit;

namespace ScanApp.Tests.UnitTests.Common.Validators
{
    public class MustContainOnlyLettersOrAllowedSymbolsValidatorTests
    {
        [Theory]
        [InlineData("Adam")]
        [InlineData("tst")]
        [InlineData("test.name")]
        [InlineData("test-name")]
        [InlineData("test_name")]
        [InlineData("test name")]
        [InlineData("__test")]
        [InlineData("new.test_name-123")]
        [InlineData("Adam.Łokietek")]
        [InlineData("Adam Łokietek")]
        public void Validates_proper_string(string data)
        {
            var subject = new MustContainOnlyLettersOrAllowedSymbolsValidator<string, string>();

            var context = new ValidationContext<string>(data);
            var result = subject.IsValid(context, data);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("tst@")]
        [InlineData("test.na|me")]
        [InlineData("test_name#")]
        [InlineData("new.test_name-123`0")]
        public void Bad_strings_are_set_as_invalid(string data)
        {
            var subject = new MustContainOnlyLettersOrAllowedSymbolsValidator<string, string>();

            var context = new ValidationContext<string>(data);
            var result = subject.IsValid(context, data);

            result.Should().BeFalse();
        }

        [Fact]
        public void Null_is_invalid()
        {
            var subject = new MustContainOnlyLettersOrAllowedSymbolsValidator<string, string>();

            var context = new ValidationContext<string>(null);
            var result = subject.IsValid(context, null);

            result.Should().BeFalse();
        }

        [Fact]
        public void Not_string_is_invalid()
        {
            var subject = new MustContainOnlyLettersOrAllowedSymbolsValidator<string, int>();

            var context = new ValidationContext<string>("test");
            var result = subject.IsValid(context, 10);

            result.Should().BeFalse();
        }
    }
}