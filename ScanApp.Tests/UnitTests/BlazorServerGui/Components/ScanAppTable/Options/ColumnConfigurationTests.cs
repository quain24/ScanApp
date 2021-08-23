using FluentAssertions;
using FluentValidation;
using ScanApp.Common;
using ScanApp.Components.ScanAppTable.Options;
using ScanApp.Tests.UnitTests.BlazorServerGui.Components.ScanAppTable.ScanAppTableTestsFixtures;
using System;
using System.Collections.Generic;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.ScanAppTable.Options
{
    public class ColumnConfigurationTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var subject = new ColumnConfiguration<ColumnConfigurationTestsFixture>(s => s.A, "name");

            subject.Should().BeOfType<ColumnConfiguration<ColumnConfigurationTestsFixture>>();
        }

        [Fact]
        public void Will_create_instance_when_given_built_in_type()
        {
            var subject = new ColumnConfiguration<string>(s => s, "string");

            subject.Should().BeOfType<ColumnConfiguration<string>>();
        }

        [Fact]
        public void Will_create_instance_if_not_given_display_name()
        {
            var subject = new ColumnConfiguration<ColumnConfigurationTestsFixture>(s => s.A, null);

            subject.Should().BeOfType<ColumnConfiguration<ColumnConfigurationTestsFixture>>();
        }

        [Fact]
        public void Will_create_instance_with_optional_parameters()
        {
            var subject = new ColumnConfiguration<ColumnConfigurationTestsFixture>(s => s.A, "A")
            {
                IsEditable = false,
                IsGroupable = false
            };

            subject.Should().BeOfType<ColumnConfiguration<ColumnConfigurationTestsFixture>>();
            Assert.False(subject.IsEditable);
            Assert.False(subject.IsGroupable);
            Assert.True(subject.IsFilterable);
        }

        [Fact]
        public void Will_throw_arg_null_exc_if_not_given_name_selector()
        {
            Action act = () => _ = new ColumnConfiguration<ColumnConfigurationTestsFixture>(null, "name");

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Will_throw_arg_exc_if_display_name_contains_only_whitespace()
        {
            Action act = () => _ = new ColumnConfiguration<ColumnConfigurationTestsFixture>(s => s.A, "   ");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Will_create_instance_with_fluent_validation_wrapper()
        {
            var subject = new ColumnConfiguration<ColumnConfigurationTestsFixture>(s => s.A, "A",
                new FluentValidationWrapper<int>(x => x.GreaterThan(10)));

            subject.Validator.Should().BeOfType<FluentValidationWrapper<int>>();
        }

        [Fact]
        public void Will_create_instance_with_fluent_validator()
        {
            var subject = new ColumnConfiguration<ColumnConfigurationTestsFixture>(s => s.A, "A",
                new ColumnConfigurationTestsValidatorFixture());

            subject.Validator.Should().BeOfType<ColumnConfigurationTestsValidatorFixture>();
        }

        [Fact]
        public void Will_transform_fluent_validation_wrapper_into_MudBlazor_compatible_field_validator()
        {
            var subject = new ColumnConfiguration<ColumnConfigurationTestsFixture>(s => s.A, "A",
                new FluentValidationWrapper<int>(x => x.GreaterThan(10)));

            var encapsulation = subject.ToMudFormFieldValidator<int>();
            encapsulation.Should().BeOfType<Func<int, IEnumerable<string>>>();
        }

        [Fact]
        public void Will_transform_abstract_validator_into_MudBlazor_compatible_field_validator()
        {
            var subject = new ColumnConfiguration<ColumnConfigurationTestsFixture>(s => s.A, "A",
                new ColumnConfigurationTestsValidatorFixture());

            var encapsulation = subject.ToMudFormFieldValidator<int>();
            encapsulation.Should().BeOfType<Func<int, IEnumerable<string>>>();
        }

        [Fact]
        public void Will_return_null_when_trying_to_transform_a_null_validator()
        {
            var subject = new ColumnConfiguration<ColumnConfigurationTestsFixture>(s => s.A, "A");

            var encapsulation = subject.ToMudFormFieldValidator<int>();
            encapsulation.Should().BeNull();
        }

        [Fact]
        public void
            Will_transform_validator_into_MudBlazor_compatible_field_validator_with_different_types()
        {
            var subject = new ColumnConfiguration<ColumnConfigurationTestsFixture>(s => s.A, "A",
                new FluentValidationWrapper<int>(x => x.GreaterThan(10)));

            var encapsulation = subject.ToMudFormFieldValidator<int?>();
            encapsulation.Should().BeOfType<Func<int?, IEnumerable<string>>>();
        }
    }
}