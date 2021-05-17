using FluentAssertions;
using FluentValidation;
using ScanApp.Common;
using ScanApp.Components.Common.ScanAppTable.Options;
using System;
using System.Collections.Generic;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.Options
{
    public class ColumnConfigTests
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
            var subject = new ColumnConfig<MyClass>(s => s.A, "name");

            subject.Should().BeOfType<ColumnConfig<MyClass>>();
        }

        [Fact]
        public void Will_create_instance_when_given_built_in_type()
        {
            var subject = new ColumnConfig<string>(s => s, "string");

            subject.Should().BeOfType<ColumnConfig<string>>();
        }

        [Fact]
        public void Will_create_instance_if_not_given_display_name()
        {
            var subject = new ColumnConfig<MyClass>(s => s.A, null);

            subject.Should().BeOfType<ColumnConfig<MyClass>>();
        }

        [Fact]
        public void Will_create_instance_with_optional_parameters()
        {
            var subject = new ColumnConfig<MyClass>(s => s.A, "A")
            {
                IsEditable = false,
                IsGroupable = false
            };

            subject.Should().BeOfType<ColumnConfig<MyClass>>();
            Assert.False(subject.IsEditable);
            Assert.False(subject.IsGroupable);
            Assert.True(subject.IsFilterable);
            Assert.True(subject.IsSelectable);
        }

        [Fact]
        public void Will_throw_arg_null_exc_if_not_given_name_selector()
        {
            Action act = () => _ = new ColumnConfig<MyClass>(null, "name");

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Will_throw_arg_exc_if_display_name_contains_only_whitespace()
        {
            Action act = () => _ = new ColumnConfig<MyClass>(s => s.A, "   ");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Will_create_instance_with_fluent_validation_wrapper()
        {
            var subject = new ColumnConfig<MyClass>(s => s.A, "A", 
                new FluentValidationWrapper<int>(x => x.GreaterThan(10)));

            subject.Validator.Should().BeOfType<Func<object, IEnumerable<string>>>();
        }

        [Fact]
        public void Will_create_instance_with_fluent_validator()
        {
            var subject = new ColumnConfig<MyClass>(s => s.A, "A", 
                new ColumnConfigTestsValidatorFixture());

            subject.Validator.Should().BeOfType<Func<object, IEnumerable<string>>>();
        }
    }
}