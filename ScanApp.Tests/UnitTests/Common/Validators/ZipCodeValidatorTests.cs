using FluentAssertions;
using FluentValidation;
using ScanApp.Common.Validators;
using Xunit;

namespace ScanApp.Tests.UnitTests.Common.Validators
{
    public class ZipCodeValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new ZipCodeValidator();

            subject.Should().NotBeNull()
                .And.BeOfType<ZipCodeValidator>()
                .Subject.Should().BeAssignableTo<AbstractValidator<string>>();
        }

        [Theory]
        [InlineData("aa-aaa")]
        [InlineData("123123")]
        [InlineData("12312-321-45")]
        [InlineData("12-345")]
        [InlineData("A21-ss")]
        public void Validates_proper_zip(string zip)
        {
            var subject = new ZipCodeValidator();

            var result = subject.Validate(zip);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Number_cannot_be_null()
        {
            var subject = new ZipCodeValidator();

            var result = subject.Validate(null as string);

            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("a")]
        [InlineData("adf12345678a1")]
        public void Length_must_be_le_12_and_ge_2(string zip)
        {
            var subject = new ZipCodeValidator();

            var result = subject.Validate(zip);

            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("-a123")]
        [InlineData("adf1234-")]
        public void Dash_cannot_be_at_first_or_last_place(string zip)
        {
            var subject = new ZipCodeValidator();

            var result = subject.Validate(zip);

            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(" a1 23")]
        [InlineData("adf1234 ")]
        public void Space_cannot_be_first_or_last_symbol(string zip)
        {
            var subject = new ZipCodeValidator();

            var result = subject.Validate(zip);

            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("  a1 23")]
        [InlineData("adf1  234")]
        [InlineData("ad   f1234")]
        public void Cannot_contain_multiple_consecutive_spaces(string zip)
        {
            var subject = new ZipCodeValidator();

            var result = subject.Validate(zip);

            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("@sssss")]
        [InlineData("123$123")]
        [InlineData("dd-123ł")]
        public void Only_ascii_letters_numbers_spaces_and_dashes_allowed(string zip)
        {
            var subject = new ZipCodeValidator();

            var result = subject.Validate(zip);

            result.IsValid.Should().BeFalse();
        }
    }
}