using FluentAssertions;
using Moq;
using ScanApp.Common;
using ScanApp.Components.Common.ScanAppTable.Options;
using System;
using FluentValidation;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.Options
{
    public class ColumnValidatorTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var subject = new ColumnValidator<string>("testName", ColumnValidatorFixtures.CreateDummyFluentValidationWrapperMock<string>().Object);

            subject.Should().BeOfType<ColumnValidator<string>>();
        }

        [Fact]
        public void Will_create_instance_with_correct_validator_and_propertyName()
        {
            var subject = new ColumnValidator<string>("testName", ColumnValidatorFixtures.CreateDummyFluentValidationWrapperMock<string>().Object);

            subject.PropertyName.Should().Be("testName");
            subject.Validator.Should().BeAssignableTo<FluentValidationWrapper<string>>();
        }

        [Fact]
        public void Will_throw_arg_exc_if_given_validator_with_not_nullable_type()
        {
            Action act = () => _ = new ColumnValidator<int>(null, ColumnValidatorFixtures.CreateDummyFluentValidationWrapperMock<int>().Object);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Will_create_instance_given_string_generic_type()
        {
            var subject = new ColumnValidator<string>("testName", ColumnValidatorFixtures.CreateDummyFluentValidationWrapperMock<string>().Object);

            subject.Should().BeOfType<ColumnValidator<string>>();
        }

        [Fact]
        public void Will_create_instance_given_nullable_DateTime_generic_type()
        {
            var subject = new ColumnValidator<DateTime?>("testName", ColumnValidatorFixtures.CreateDummyFluentValidationWrapperMock<DateTime?>().Object);

            subject.Should().BeAssignableTo<ColumnValidator<DateTime?>>();
        }

        [Fact]
        public void Will_throw_arg_null_exc_if_not_given_property_name()
        {
            Action act = () => _ = new ColumnValidator<string>(null, ColumnValidatorFixtures.CreateDummyFluentValidationWrapperMock<string>().Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Will_throw_arg_exc_if_given_property_name_with_whitespaces_only()
        {
            Action act = () => _ = new ColumnValidator<string>("    ", Mock.Of<FluentValidationWrapper<string>>());

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Will_throw_arg_null_exc_if_all_parameters_are_null()
        {
            Action act = () => _ = new ColumnValidator<string>(null, null);

            act.Should().Throw<ArgumentNullException>();
        }
    }
}