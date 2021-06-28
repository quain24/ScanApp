﻿using FluentAssertions;
using FluentValidation;
using ScanApp.Common;
using ScanApp.Components.Common.ScanAppTable.Options;
using System;
using System.Collections.Generic;
using ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.ScanAppTableTestsFixtures;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.Options
{
    public class ColumnConfigurationTests
    {
        public class MyClass
        {
            public string A { get; }
            public int B { get; }

            public MyClass(string a, int b)
            {
                A = a;
                B = b;
            }
        }

        [Fact]
        public void Will_create_instance()
        {
            var subject = new ColumnConfiguration<MyClass>(s => s.A, "name");

            subject.Should().BeOfType<ColumnConfiguration<MyClass>>();
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
            var subject = new ColumnConfiguration<MyClass>(s => s.A, null);

            subject.Should().BeOfType<ColumnConfiguration<MyClass>>();
        }

        [Fact]
        public void Will_create_instance_with_optional_parameters()
        {
            var subject = new ColumnConfiguration<MyClass>(s => s.A, "A")
            {
                IsEditable = false,
                IsGroupable = false
            };

            subject.Should().BeOfType<ColumnConfiguration<MyClass>>();
            Assert.False(subject.IsEditable);
            Assert.False(subject.IsGroupable);
            Assert.True(subject.IsFilterable);
            Assert.True(subject.IsSelectable);
        }

        [Fact]
        public void Will_throw_arg_null_exc_if_not_given_name_selector()
        {
            Action act = () => _ = new ColumnConfiguration<MyClass>(null, "name");

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Will_throw_arg_exc_if_display_name_contains_only_whitespace()
        {
            Action act = () => _ = new ColumnConfiguration<MyClass>(s => s.A, "   ");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Will_create_instance_with_fluent_validation_wrapper()
        {
            var subject = new ColumnConfiguration<MyClass>(s => s.A, "A", 
                new FluentValidationWrapper<int>(x => x.GreaterThan(10)));

            subject.Validator.Should().BeOfType<FluentValidationWrapper<int>>();
        }

        [Fact]
        public void Will_create_instance_with_fluent_validator()
        {
            var subject = new ColumnConfiguration<MyClass>(s => s.A, "A", 
                new ColumnConfigTestsValidatorFixture());

            subject.Validator.Should().BeOfType<ColumnConfigTestsValidatorFixture>();
        }

        [Fact]
        public void Will_transform_fluent_validation_wrapper_into_MudBlazor_compatible_field_validator()
        {
            var subject = new ColumnConfiguration<MyClass>(s => s.A, "A", 
                new FluentValidationWrapper<int>(x => x.GreaterThan(10)));

            var encapsulation = subject.ToMudFormFieldValidator<int>();
            encapsulation.Should().BeOfType<Func<int, IEnumerable<string>>>();
        }

        [Fact]
        public void Will_transform_abstract_validator_into_MudBlazor_compatible_field_validator()
        {
            var subject = new ColumnConfiguration<MyClass>(s => s.A, "A", 
                new ColumnConfigTestsValidatorFixture());

            var encapsulation = subject.ToMudFormFieldValidator<int>();
            encapsulation.Should().BeOfType<Func<int, IEnumerable<string>>>();
        }

        [Fact]
        public void Will_return_null_when_trying_to_transform_a_null_validator()
        {
            var subject = new ColumnConfiguration<MyClass>(s => s.A, "A");

            var encapsulation = subject.ToMudFormFieldValidator<int>();
            encapsulation.Should().BeNull();
        }

        [Fact]
        public void
            Will_transform_validator_into_MudBlazor_compatible_field_validator_with_different_types()
        {
            var subject = new ColumnConfiguration<MyClass>(s => s.A, "A", 
                new FluentValidationWrapper<int>(x => x.GreaterThan(10)));

            var encapsulation = subject.ToMudFormFieldValidator<int?>();
            encapsulation.Should().BeOfType<Func<int?, IEnumerable<string>>>();
        }
    }
}