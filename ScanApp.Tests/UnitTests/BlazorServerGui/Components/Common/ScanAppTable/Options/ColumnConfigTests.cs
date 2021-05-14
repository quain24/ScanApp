using FluentAssertions;
using ScanApp.Components.Common.ScanAppTable.Options;
using System;
using FluentValidation;
using ScanApp.Common;
using ScanApp.Common.Validators;
using ScanApp.Models.SpareParts;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.Options
{
    public class ColumnConfigTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var subject = new ColumnConfig<OptionsFixtures>(s => s.B, "name");

            subject.Should().BeOfType<ColumnConfig<OptionsFixtures>>();
        }


        [Fact]
        public void Will_create_instance_if_not_given_display_name()
        {
            var subject = new ColumnConfig<OptionsFixtures>(s => s.B, null);

            subject.Should().BeOfType<ColumnConfig<OptionsFixtures>>();
        }

        [Fact]
        public void Will_create_instance_when_given_value_type()
        {
            var subject = new ColumnConfig<int>(s => s, "int");

            subject.Should().BeOfType<ColumnConfig<int>>();
        }

        [Fact]
        public void  Will_have_correct_property_type_when_name_selector_value_type()
        {
            var subject = new ColumnConfig<double>(s => s, "str");

            subject.PropertyType.Should().Be(typeof(double));
        }

        [Fact]
        public void  Will_have_correct_property_type_when_name_selector_chooses_model_property()
        {
            var subject = new ColumnConfig<OptionsFixtures>(s => s.A, null);

            subject.PropertyType.Should().Be(typeof(string));
        }

        [Fact]
        public void  Will_have_correct_property_name_when_name_selector_chooses_model_property()
        {
            var subject = new ColumnConfig<OptionsFixtures>(s => s.A, null);

            subject.PropertyName.Should().Be(nameof(OptionsFixtures.A));
        }

        [Fact]
        public void Will_have_correct_property_type_when_name_selector_chooses_sub_level_property()
        {
            var subject = new ColumnConfig<OptionsFixtures>(s => s.D.Name, null);

            subject.PropertyType.Should().Be(typeof(string));
        }

        [Fact]
        public void Will_have_correct_property_name_when_name_selector_chooses_sub_level_property()
        {
            var subject = new ColumnConfig<OptionsFixtures>(s => s.D.Name, null);

            subject.PropertyName.Should().Be(nameof(OptionsFixtures.D.Name));
        }

        [Fact]
        public void Will_have_correct_property_type_when_name_selector_chooses_type()
        {
            var subject = new ColumnConfig<OptionsFixtures>(s => s.D, null);

            subject.PropertyType.Should().Be(typeof(SparePartGUIModel));
        }

        [Fact]
        public void Will_have_correct_property_name_when_name_selector_chooses_type()
        {
            var subject = new ColumnConfig<OptionsFixtures>(s => s.D, null);

            subject.PropertyName.Should().Be(nameof(OptionsFixtures.D));
        }

        [Fact]
        public void Will_create_instance_with_optional_parameters()
        {
            var subject = new ColumnConfig<OptionsFixtures>(s => s.A, "A")
            {
                IsEditable = false,
                IsGroupable = false
            };

            subject.Should().BeOfType<ColumnConfig<OptionsFixtures>>();
            subject.IsEditable.Should().BeFalse();
            subject.IsGroupable.Should().BeFalse();
            subject.IsFilterable.Should().BeTrue();
            subject.IsSelectable.Should().BeTrue();
        }

        [Fact]
        public void Will_throw_arg_null_exc_if_not_given_name_selector()
        {
            Action act = () => _ = new ColumnConfig<OptionsFixtures>(null, "name");

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Will_throw_arg_exc_if_display_name_contains_only_whitespace()
        {
            Action act = () => _ = new ColumnConfig<OptionsFixtures>(s => s.A, "   ");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Will_create_instance_with_proper_validator()
        {
            var validator = new FluentValidationWrapper<string>(x => x.Cascade(CascadeMode.Stop).Length(10));
            var val = new IdentityNamingValidator<string, string>();
            var subject = new ColumnConfig<OptionsFixtures>(s => s.A, "A", new Vall());
            var a = subject.Validator("aaa");
            var x = "";
        }
    }

    public class Vall : AbstractValidator<string>
    {
        public Vall()
        {
            RuleFor(c => c).Length(10);
        }
    }
}