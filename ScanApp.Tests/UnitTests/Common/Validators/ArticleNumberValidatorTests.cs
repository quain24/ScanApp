using FluentAssertions;
using ScanApp.Common.Validators;
using Xunit;

namespace ScanApp.Tests.UnitTests.Common.Validators
{
    public class ArticleNumberValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new ArticleNumberValidator();

            subject.Should().NotBeNull().And.BeOfType<ArticleNumberValidator>();
        }

        [Fact]
        public void Result_cannot_be_null()
        {
            var subject = new ArticleNumberValidator();

            var result = subject.Validate(null as string);

            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("123 456")]
        [InlineData("123456")]
        [InlineData("12345")]
        [InlineData("12345678912345678912")]
        [InlineData("123 456 789 12345678")]
        [InlineData("abc 456 def 12345678")]
        public void Result_is_valid_for_proper_numbers(string number)
        {
            var subject = new ArticleNumberValidator();

            var result = subject.Validate(number);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        public void Result_cannot_be_empty(string number)
        {
            var subject = new ArticleNumberValidator();

            var result = subject.Validate(number);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Result_is_invalid_if_length_is_lt_5()
        {
            var subject = new ArticleNumberValidator();

            var result = subject.Validate("1234");

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Result_is_invalid_if_length_is_gt_20()
        {
            var subject = new ArticleNumberValidator();

            var result = subject.Validate("123456789123456789123");

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Result_is_invalid_if_it_starts_with_whitespace()
        {
            var subject = new ArticleNumberValidator();

            var result = subject.Validate(" 12345");

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Result_is_invalid_if_it_ends_with_whitespace()
        {
            var subject = new ArticleNumberValidator();

            var result = subject.Validate("12345 ");

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Result_is_invalid_if_contains_many_whitespaces_consecutively()
        {
            var subject = new ArticleNumberValidator();

            var result = subject.Validate("12345  456");

            result.IsValid.Should().BeFalse();
        }
    }
}