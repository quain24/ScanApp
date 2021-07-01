using FluentAssertions;
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
        [InlineData("__test:")]
        [InlineData("__te\"st")]
        [InlineData("new.test_name-123")]
        [InlineData("/test")]
        [InlineData("Adam.Łokietek'")]
        [InlineData("Adam Łokietek")]
        public void Validates_proper_string(string data)
        {
            var subject = new MustContainOnlyLettersOrAllowedSymbolsValidator();

            var result = subject.Validate(data);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("tst@")]
        [InlineData("test.na|me")]
        [InlineData("test_name#")]
        [InlineData("new.test_name-123`0")]
        public void Bad_strings_are_set_as_invalid(string data)
        {
            var subject = new MustContainOnlyLettersOrAllowedSymbolsValidator();

            var result = subject.Validate(data);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Null_is_invalid()
        {
            var subject = new MustContainOnlyLettersOrAllowedSymbolsValidator();

            var result = subject.Validate(null as string);

            result.IsValid.Should().BeFalse();
        }
    }
}