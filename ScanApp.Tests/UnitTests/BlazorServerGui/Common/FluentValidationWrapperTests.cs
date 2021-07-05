using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using Moq;
using ScanApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Common
{
    public class FluentValidationWrapperTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var subject = new FluentValidationWrapper<int>(x => x.Equal(1), false, "is null");

            subject.Should().BeOfType<FluentValidationWrapper<int>>()
                .And.BeAssignableTo<AbstractValidator<int>>();
        }

        [Fact]
        public void Will_throw_arg_null_exc_if_not_given_any_rules()
        {
            Action act = () => _ = new FluentValidationWrapper<int>(null, false, "is null");

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Will_accept_rules_when_created()
        {
            var subject = new FluentValidationWrapper<int>(x => x.GreaterThan(1).LessThan(100), false, "is null");

            var result = subject.CreateDescriptor();
            result.Rules.Should().HaveCount(1, "there is only one property being validated");
            var rules = result.Rules.First().Components;
            rules.Should().HaveCount(2, "two validators were given for property being validated")
                .And.Subject.Should().ContainSingle(v => v.Validator.GetType() == typeof(LessThanValidator<int, int>))
                .And.Subject.Should().ContainSingle(v => v.Validator.GetType() == typeof(GreaterThanValidator<int, int>));
        }

        [Fact]
        public void Validation_creates_actual_validating_delegate()
        {
            var subject = new FluentValidationWrapper<string>(x => x.MaximumLength(1), false, "is null");

            var result = subject.Validation;

            result.Should().BeOfType<Func<string, IEnumerable<string>>>()
                .And.NotBeNull();
        }

        [Fact]
        public void Will_invalidate_if_given_property_is_null_with_given_error_text()
        {
            var subject = new FluentValidationWrapper<string>(x => x.MaximumLength(1), false, "is null");

            var action = subject.Validation;
            var result = action(null);

            result.Should().HaveCount(1)
                .And.Subject.First().Should().Be("is null");
        }

        [Fact]
        public void Returns_empty_string_collection_if_given_property_is_valid()
        {
            var subject = new FluentValidationWrapper<string>(x => x.MaximumLength(10), false, "is null");

            var action = subject.Validation;
            var result = action("12345");

            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_string_collection_of_error_messages_if_given_property_is_invalid()
        {
            var validatorMock = new Mock<FluentValidationWrapper<string>>(Mock.Of<Action<IRuleBuilderInitial<string, string>>>(), false, "");
            validatorMock.Setup(m => m.Validate(It.IsAny<ValidationContext<string>>()))
                .Returns(new ValidationResult(new[] { new ValidationFailure("name", "message") }));

            var action = validatorMock.Object.Validation;
            var result = action("invalid data");

            result.Should().HaveCount(1)
                .And.Subject.First().Should().Be("message");
        }
    }
}