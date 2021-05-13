using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using ScanApp.Common;
using ScanApp.Components.Common.ScanAppTable.Options;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.Options
{
    public class ColumnValidatorTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var subject = new ColumnValidator<string>("testName",
                new FluentValidationWrapper<string>(x => x.MinimumLength(10)));

            subject.Should().BeOfType<ColumnValidator<string>>();
        }

        [Fact]
        public void Will_create_instance_with_correct_validator_and_propertyName()
        {
            var subject = new ColumnValidator<string>("testName",
                new FluentValidationWrapper<string>(x => x.MinimumLength(10)));

            subject.PropertyName.Should().Be("testName");
            subject.Validator.Should().BeOfType<FluentValidationWrapper<string>>();
        }

        [Fact]
        public void Will_throw_arg_exc_if_given_validator_with_not_nullable_type()
        {
            Action act = () => _ = new ColumnValidator<int>(null,
                new FluentValidationWrapper<int>(x => x.GreaterThan(0)));

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Will_not_throw_arg_exc_if_given_validator_with_string_type()
        {
            var subject = new ColumnValidator<string>("testName",
                new FluentValidationWrapper<string>(x => x.MinimumLength(0)));

            subject.Should().BeOfType<ColumnValidator<string>>();
        }

        [Fact]
        public void Will_not_throw_arg_exc_if_given_validator_with_nullable_DateTime_type()
        {
            var subject = new ColumnValidator<DateTime?>("testName",
                new FluentValidationWrapper<DateTime?>(x => x.Empty()));

            subject.Should().BeOfType<ColumnValidator<DateTime?>>();
        }

        [Fact]
        public void Will_throw_arg_null_exc_if_not_given_property_name()
        {
            Action act = () => _ = new ColumnValidator<string>(null,
                new FluentValidationWrapper<string>(x => x.MinimumLength(10)));

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Will_throw_arg_exc_if_given_property_name_with_whitespaces_only()
        {
            Action act = () => _ = new ColumnValidator<string>("    ",
                new FluentValidationWrapper<string>(x => x.MinimumLength(10)));

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
