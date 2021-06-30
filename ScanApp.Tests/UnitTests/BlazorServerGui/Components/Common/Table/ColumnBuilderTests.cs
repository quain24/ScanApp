using FluentAssertions;
using FluentValidation;
using Moq;
using ScanApp.Components.Common.Table;
using ScanApp.Tests.UnitTests.BlazorServerGui.Services;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using MudBlazor;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.Table
{
    public class ColumnBuilderTests
    {
        [Fact]
        public void Builds_with_name_only_as_col_conf_set_as_presenter()
        {
            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .ForPresentation("name")
                .Build();

            subject.DisplayName.Should().Be("name");
        }

        [Fact] public void Builds_with_name_and_column_style_as_col_conf_set_as_presenter()
        {
            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .ForPresentation("name")
                .ColumnStyle("color: red;")
                .Build();

            subject.DisplayName.Should().Be("name");
            subject.ColumnStyle.Should().Be("color: red;");
        }

        [Fact]
        public void Builds_with_given_target()
        {
            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .For(x => x.SubClassProp.AString)
                .Build();

            subject.PropertyType.Should().Be(typeof(string));
            subject.DisplayName.Should().Be(nameof(ColumnConfigFixtures.TestObject.SubClassProp.AString));
        }

        [Fact]
        public void Builds_with_given_name()
        {
            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .For(x => x)
                .UnderName("test name")
                .Build();

            subject.DisplayName.Should().Be("test name");
        }

        [Fact] public void Builds_as_editable()
        {
            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .For(x => x)
                .Editable()
                .Build();

            subject.IsEditable.Should().BeTrue();
        }

        [Fact]
        public void Builds_as_groupable()
        {
            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .For(x => x)
                .Groupable()
                .Build();

            subject.IsGroupable.Should().BeTrue();
        }

        [Fact]
        public void Builds_as_non_filterable()
        {
            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .For(x => x)
                .DisableFiltering()
                .Build();

            subject.IsFilterable.Should().BeFalse();
        }

        [Fact]
        public void Builds_with_validation()
        {
            var val = Mock.Of<IValidator<PropertyPathTestsFixtures.TestObject>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()));

            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .For(x => x)
                .ValidateUsing<PropertyPathTestsFixtures.TestObject>(val)
                .Build();

            subject.IsValidatable().Should().BeTrue();
        }

        [Fact]
        public void Builds_with_given_format()
        {
            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .For(x => x)
                .FormatAs(FieldType.DateAndTime)
                .Build();

            subject.FieldType.Should().Be(FieldType.DateAndTime);
        }

        [Fact]
        public void Builds_with_converter()
        {
            var converter = Mock.Of<MudBlazor.Converter<string>>();
            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .For(x => x.AString)
                .ConvertUsing(converter)
                .Build();

            ((object)subject.Converter).Should().NotBeNull()
                .And.BeAssignableTo<MudBlazor.Converter<string>>();
        }

        [Fact]
        public void Builds_with_given_limiting_value_set()
        {
            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .For(x => x.AString)
                .LimitValuesTo(new List<string>{ "a", "B"})
                .Build();

            subject.AllowedValues.Should().BeEquivalentTo(new List<string> {"a", "B"});
        }
    }
}