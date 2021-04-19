using FluentAssertions;
using FluentValidation;
using ScanApp.Common.Validators;
using System;
using System.Linq;
using Xunit;

namespace ScanApp.Tests.UnitTests.Common.Validators
{
    public class IdentityNamingValidatorTests
    {
        private const string Chars = "abcdefghijklmnopqrstuvwxyz0123456789._-";

        [Theory]
        [InlineData("Adam")]
        [InlineData("tst")]
        [InlineData("test.name")]
        [InlineData("test-name")]
        [InlineData("test_name")]
        [InlineData("__test")]
        [InlineData("new.test_name-123")]
        public void Validates_proper_string(string data)
        {
            var subject = new IdentityNamingValidator<string, string>();

            var context = new ValidationContext<string>(data);
            var result = subject.IsValid(context, data);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("Adam ")]
        [InlineData("tst@")]
        [InlineData("test.na|me")]
        [InlineData("test - name")]
        [InlineData("test_name#")]
        [InlineData("Adam.Łokietek")]
        [InlineData("12")]
        [InlineData("new.test_name-123`0")]
        public void Bad_strings_are_set_as_invalid(string data)
        {
            var subject = new IdentityNamingValidator<string, string>();

            var context = new ValidationContext<string>(data);
            var result = subject.IsValid(context, data);

            result.Should().BeFalse();
        }

        [Fact]
        public void Validates_name_with_maximum_length()
        {
            var subject = new IdentityNamingValidator<string, string>();
            var random = new Random(1);
            var data = new string(Enumerable.Repeat(Chars, 450)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());

            var context = new ValidationContext<string>(data);
            var result = subject.IsValid(context, data);

            result.Should().BeTrue();
        }

        [Fact]
        public void Invalid_if_data_is_too_long()
        {
            var subject = new IdentityNamingValidator<string, string>();
            var random = new Random(1);
            var data = new string(Enumerable.Repeat(Chars, 451)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());

            var context = new ValidationContext<string>(data);
            var result = subject.IsValid(context, data);

            result.Should().BeFalse();
        }

        [Fact]
        public void Null_is_invalid()
        {
            var subject = new IdentityNamingValidator<string, string>();

            var context = new ValidationContext<string>(null);
            var result = subject.IsValid(context, null);

            result.Should().BeFalse();
        }

        [Fact]
        public void Not_string_is_invalid()
        {
            var subject = new IdentityNamingValidator<string, int>();

            var context = new ValidationContext<string>("test");
            var result = subject.IsValid(context, 10);

            result.Should().BeFalse();
        }
    }
}