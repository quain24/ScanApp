﻿using FluentAssertions;
using FluentValidation;
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
            var context = new ValidationContext<string>(phone);
            var subject = new PhoneNumberValidator<string, string>();

            var result = subject.IsValid(context, phone);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("123-456-789")]
        [InlineData("(00)12347852587")]
        [InlineData("+ 01234785258789621485")]
        [InlineData("+48 123 789 621 485")]
        [InlineData("123456D")]
        public void Invalidates_wrong_phone_number(string phone)
        {
            var context = new ValidationContext<string>(phone);
            var subject = new PhoneNumberValidator<string, string>();

            var result = subject.IsValid(context, phone);

            result.Should().BeFalse();
        }

        [Fact]
        public void Returns_false_if_phone_is_null()
        {
            var subject = new PhoneNumberValidator<string, string>();

            var context = new ValidationContext<string>("context");
            var result = subject.IsValid(context, null);

            result.Should().BeFalse();
        }

        [Fact]
        public void Returns_false_if_email_is_not_a_string()
        {
            var subject = new PhoneNumberValidator<string, int>();

            var context = new ValidationContext<string>("context");
            var result = subject.IsValid(context, 1);

            result.Should().BeFalse();
        }
    }
}