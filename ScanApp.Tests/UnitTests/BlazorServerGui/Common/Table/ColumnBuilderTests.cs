using FluentAssertions;
using FluentValidation;
using Moq;
using ScanApp.Components.Common.Table;
using ScanApp.Tests.UnitTests.BlazorServerGui.Services;
using System;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Common.Table
{
    public class ColumnBuilderTests
    {
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

        [Fact]
        public void Builds_as_readonly()
        {
            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .For(x => x)
                .AsReadOnly()
                .Build();

            subject.IsEditable.Should().BeFalse();
        }

        [Fact]
        public void Builds_as_non_groupable()
        {
            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .For(x => x)
                .DisableGrouping()
                .Build();

            subject.IsGroupable.Should().BeFalse();
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
            var val = Mock.Of<IValidator>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()));

            var subject = ColumnBuilder<PropertyPathTestsFixtures.TestObject>
                .For(x => x)
                .ValidateUsing(val)
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
    }
}