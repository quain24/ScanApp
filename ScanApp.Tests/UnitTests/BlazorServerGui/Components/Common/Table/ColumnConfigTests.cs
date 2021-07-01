using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using MudBlazor;
using ScanApp.Common;
using ScanApp.Components.Common.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Xunit.Abstractions;
using static ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.Table.ColumnConfigFixtures;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.Table
{
    public class ColumnConfigTests
    {
        public ITestOutputHelper Output { get; }

        public ColumnConfigTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Creates_instance_when_only_display_name_is_given_configured_as_presenter()
        {
            var subject = new ColumnConfig<TestObject>("name");

            subject.Should().BeOfType<ColumnConfig<TestObject>>();
            subject.IsPresenter.Should().BeTrue();
            subject.DisplayName.Should().Be("name");
        }

        [Fact]
        public void Throws_arg_null_exc_if_tried_to_create_presenter_col_conf_with_null_name()
        {
            Action act = () => _ = new ColumnConfig<TestObject>((string)null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_null_exc_if_no_target_is_given_on_initialization()
        {
            Action act = () => _ = new ColumnConfig<TestObject>((Expression<Func<TestObject, dynamic>>)null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(AutoDisplayNameFixture))]
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        public void Sets_display_name_from_given_parameter(Expression<Func<TestObject, object>> target, string _)
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
        {
            var subject = new ColumnConfig<TestObject>(target, "given name");

            subject.DisplayName.Should().BeEquivalentTo("given name");
        }

        [Theory]
        [ClassData(typeof(AutoDisplayNameFixture))]
        public void Uses_target_property_or_field_name_if_no_display_name_is_given(Expression<Func<TestObject, object>> target, string expected)
        {
            var subject = new ColumnConfig<TestObject>(target);

            subject.DisplayName.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(FieldType.Date)]
        [InlineData(FieldType.Time)]
        [InlineData(FieldType.DateAndTime)]
        [InlineData(FieldType.AutoDetect)]
        [InlineData(FieldType.PlainText)]
        public void Uses_given_FieldType(FieldType type)
        {
            var subject = new ColumnConfig<TestObject>(c => c, null, type);

            subject.FieldType.Should().Be(type);
        }

        [Fact]
        public void Given_no_FieldType_uses_FieldType_AutoDetect()
        {
            var subject = new ColumnConfig<TestObject>(c => c);

            subject.FieldType.Should().Be(FieldType.AutoDetect);
        }

        [Fact]
        public void Given_input_limiting_collection_will_contain_it()
        {
            var subject = new ColumnConfig<TestObject>(c => c.AString).LimitAcceptedValuesTo(new List<string> { "a", "b", "c" });

            subject.AllowedValues.Should().BeEquivalentTo(new List<string> { "a", "b", "c" });
        }

        [Fact]
        public void Throws_arg_null_exc_if_given_limiting_collection_was_null()
        {
            Action act = () => _ = new ColumnConfig<TestObject>(x => x.AString).LimitAcceptedValuesTo(null as IEnumerable<string>);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_exc_if_given_limiting_collection_item_types_are_not_compatible_with_target_type()
        {
            Action act = () => _ = new ColumnConfig<TestObject>(x => x.AString).LimitAcceptedValuesTo(new List<int> { 1, 2, 3 });

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [ClassData(typeof(DetectTypeFixture))]
        public void Detects_type_of_object_pointed_to_by_target_expression(Expression<Func<TestObject, object>> target, Type expected)
        {
            var subject = new ColumnConfig<TestObject>(target);

            subject.PropertyType.Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(ProperValidatorTypeFixture))]
        public void Accepts_proper_validator(Expression<Func<TestObject, object>> target, dynamic mock)
        {
            Action act = () => _ = new ColumnConfig<TestObject>(target, null).AssignValidator(mock);

            act.Should().NotThrow();
        }

        [Theory]
        [ClassData(typeof(ProperValidatorInvalidTypeFixture))]
        public void Throws_arg_exc_if_given_validator_cannot_validate_type_pointed_by_target(Expression<Func<TestObject, object>> target, dynamic mock)
        {
            Action act = () => _ = new ColumnConfig<TestObject>(target, null).AssignValidator(mock);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void IsValidatable_returns_true_if_there_is_proper_validator()
        {
            var validatorMock = new Mock<IValidator<int>>();
            validatorMock.Setup(v => v.CanValidateInstancesOfType(It.IsAny<Type>())).Returns(true);
            validatorMock.Setup(v => v.CreateDescriptor()).Returns(new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>()));
            var subject = new ColumnConfig<TestObject>(c => c.AnInt, null).AssignValidator(validatorMock.Object);

            subject.IsValidatable().Should().BeTrue();
        }

        [Fact]
        public void IsValidatable_returns_true_if_there_is_proper_validator_and_given_type_match()
        {
            var validatorMock = new Mock<IValidator<int>>();
            validatorMock.Setup(v => v.CanValidateInstancesOfType(It.Is<Type>(c => c == typeof(int)))).Returns(true);
            validatorMock.Setup(v => v.CreateDescriptor()).Returns(new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>()));
            var subject = new ColumnConfig<TestObject>(c => c.AnInt, null).AssignValidator(validatorMock.Object);

            subject.IsValidatable(typeof(int)).Should().BeTrue();
        }

        [Fact]
        public void IsValidatable_returns_false_if_there_is_proper_validator_but_given_type_mismatch()
        {
            var validatorMock = new Mock<IValidator<string>>();
            validatorMock.Setup(v => v.CanValidateInstancesOfType(It.Is<Type>(c => c == typeof(string)))).Returns(true);
            validatorMock.Setup(v => v.CreateDescriptor()).Returns(new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>()));
            var subject = new ColumnConfig<TestObject>(c => c.AString, null).AssignValidator(validatorMock.Object);

            subject.IsValidatable(typeof(int)).Should().BeFalse();
        }

        [Fact]
        public void IsValidatable_returns_false_if_there_is_no_validator()
        {
            var subject = new ColumnConfig<TestObject>(c => c.AnInt);

            subject.IsValidatable().Should().BeFalse();
        }

        [Fact]
        public void IsValidatable_returns_false_if_there_is_no_validator_but_given_type_match()
        {
            var subject = new ColumnConfig<TestObject>(c => c.AString);

            subject.IsValidatable(typeof(string)).Should().BeFalse();
        }

        [Fact]
        public void Validate_will_perform_validation()
        {
            var data = new TestObject { AString = "wowww" };
            var validatorMock = new Mock<IValidator<string>>();
            validatorMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(new ValidationResult());
            validatorMock.Setup(v => v.CreateDescriptor()).Returns(new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>()));
            validatorMock.Setup(v => v.CanValidateInstancesOfType(It.Is<Type>(c => c == typeof(string)))).Returns(true);
            var subject = new ColumnConfig<TestObject>(c => c.AString, null).AssignValidator(validatorMock.Object);

            var result = subject.Validate(data.AString);

            result.Should().BeEmpty();
        }

        [Fact]
        public void Validate_gives_list_of_errors_if_invalid()
        {
            var data = new TestObject { AString = "wowww" };
            var validatorMock = new Mock<IValidator<string>>();
            var failure1 = new ValidationFailure("prop name 1", "error 1");
            var failure2 = new ValidationFailure("prop name 1", "error 2");
            validatorMock.Setup(v => v.Validate(It.IsAny<string>()))
                .Returns(new ValidationResult(new[] { failure1, failure2 }));
            validatorMock.Setup(v => v.CanValidateInstancesOfType(It.Is<Type>(c => c == typeof(string)))).Returns(true);
            validatorMock.Setup(v => v.CreateDescriptor()).Returns(new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>()));
            var subject = new ColumnConfig<TestObject>(c => c.AString, null).AssignValidator(validatorMock.Object);

            var result = subject.Validate(data.AString);

            result.Should().HaveCount(2).And.Contain(failure1.ErrorMessage).And.Contain(failure2.ErrorMessage);
        }

        [Fact]
        public void Validate_throws_invalid_operation_exc_on_invalid_type_given_to_validate()
        {
            var data = new TestObject { AString = "wowww" };
            var validator = new FluentValidationWrapper<int>(x => x.LessThan(10));
            var subject = new ColumnConfig<TestObject>(c => c.AnInt, null).AssignValidator(validator);

            Action act = () => _ = subject.Validate(data.AString);

            act.Should().Throw<InvalidCastException>();
        }

        [Fact]
        public void SetValue_throws_arg_null_exc_when_target_is_null()
        {
            var config = new ColumnConfig<ColumnConfigFixtures.TestObject>(target => target.AnInt);
            Action act = () => config.SetValue(null, 1);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetValue_throws_arg_exc_when_target_has_different_type_than_value()
        {
            var target = new TestObject();
            var config = new ColumnConfig<TestObject>(t => t.AnInt);
            Action act = () => config.SetValue(target, "a");

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [ClassData(typeof(ColumnConfigExtensionsTheoryData))]
        public void SetValue_sets_value(TestObject initialTargetState, Expression<Func<TestObject, object>> expr, dynamic value, TestObject expected)
        {
            var target = initialTargetState;

            var config = new ColumnConfig<TestObject>(expr);
            TestObject result = config.SetValue(target, value);

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [ClassData(typeof(PrimitiveTheoryDataInt))]
        public void SetValue_sets_value_in_primitive(int initialTargetState, Expression<Func<int, object>> expr, int value, int expected)
        {
            var target = initialTargetState;
            var config = new ColumnConfig<int>(expr);
            var result = config.SetValue(target, value);

            result.Should().Be(expected);
        }

        [Fact]
        public void SetValue_throws_arg_exc_when_trying_to_replace_value_in_struct()
        {
            var target = new TestStruct();
            var sut = new ColumnConfig<TestStruct>(t => t.IntVal);

            Action act = () => _ = sut.SetValue(target, 10);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void SetValue_throws_arg_exc_when_trying_to_replace_value_in_struct_nested_inside_struct()
        {
            var target = new TestStruct() { StructVal = new SubStruct() { IntVal = 1 }, IntVal = 12 };
            var sut = new ColumnConfig<TestStruct>(t => t.StructVal.IntVal);

            Action act = () => _ = sut.SetValue(target, 10);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void SetValue_throws_arg_exc_when_trying_to_replace_reference_type_in_struct()
        {
            var target = new TestStruct() { ClassVal = new SubClass() { AString = "test" } };
            var sut = new ColumnConfig<TestStruct>(t => t.ClassVal);

            Action act = () => _ = sut.SetValue(target, new SubClass() { AString = "wont happen" });

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [ClassData(typeof(StructTheoryData))]
        public void SetValue_sets_value_in_struct(TestStruct initialTargetState, Expression<Func<TestStruct, object>> expr, dynamic value, TestStruct expected)
        {
            var target = initialTargetState;
            var config = new ColumnConfig<TestStruct>(expr);
            TestStruct result = config.SetValue(target, value);

            result.Should().BeEquivalentTo(expected, o =>
            {
                o.ComparingByMembers<TestStruct>();
                return o;
            });
        }

        [Theory]
        [ClassData(typeof(InvalidStructTheoryData))]
        public void SetValue_throws_when_modifying_insides_of_struct_that_are_not_reference_type(TestStruct initialTargetState, Expression<Func<TestStruct, object>> expr, dynamic value, TestStruct expected)
        {
            var target = initialTargetState;
            var config = new ColumnConfig<TestStruct>(expr);
            Action act = () => _ = config.SetValue(target, value);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void SetValue_throws_arg_exc_when_trying_to_write_to_target_when_target_is_method_call()
        {
            var target = new TestObject { SubClassField = new SubClass() { AString = "wow" } };
            var sut = new ColumnConfig<TestObject>(t => t.TestMethod());

            Action act = () => _ = sut.SetValue(target, 10);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void SetValue_throws_arg_exc_when_trying_to_write_to_read_only_target()
        {
            var target = new TestObject { SubClassField = new SubClass() { AString = "wow" } };
            var sut = new ColumnConfig<TestObject>(t => t.Readonlyint);

            Action act = () => _ = sut.SetValue(target, 10);

            act.Should().Throw<Exception>();
        }

        [Theory]
        [ClassData(typeof(GetValueProperTheoryData))]
        public void GetValueFrom_retrieves_value_from_target(TestObject source, Expression<Func<TestObject, object>> target, dynamic expected)
        {
            var sut = new ColumnConfig<TestObject>(target);

            Output.WriteLine(sut.PropertyName + " " + sut.PropertyType);
            ((object)sut.GetValueFrom(source)).Should().BeEquivalentTo((object)expected);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(10)]
        [InlineData(-12)]
        public void GetValueFrom_retrieves_data_when_target_is_direct_primitive(int data)
        {
            var source = data;
            var subject = new ColumnConfig<int>(x => x);

            var result = subject.GetValueFrom(source);

            ((int)result).Should().Be(data);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("test")]
        [InlineData("    ")]
        public void GetValueFrom_retrieves_data_when_target_is_direct_primitive_string(string data)
        {
            var source = data;
            var subject = new ColumnConfig<string>(x => x);

            var result = subject.GetValueFrom(source);

            ((string)result).Should().Be(data);
        }

        [Fact]
        public void AssignConverter_accepts_and_sets_valid_converter()
        {
            var subject = new ColumnConfig<TestObject>(c => c.AString);
            Action act = () => subject.AssignConverter(new Converter<string>());

            act.Should().NotThrow();
            ((object)subject.Converter).Should().BeOfType<Converter<string>>()
                .And.NotBeNull();
        }

        [Fact]
        public void AssignConverter_throws_arg_null_exc_when_given_converter_is_null()
        {
            var subject = new ColumnConfig<TestObject>(c => c.AString);
            Action act = () => subject.AssignConverter(null as Converter<string>);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AssignConverter_throws_arg_exc_when_given_converter_does_not_match_targets_type()
        {
            var subject = new ColumnConfig<TestObject>(c => c.AString);
            Action act = () => subject.AssignConverter(new Converter<int>());

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void LimitAcceptedValues_accepts_valid_collection()
        {
            var subject = new ColumnConfig<TestObject>(c => c.AString);
            Action act = () => subject.LimitAcceptedValuesTo(new string[] { "a", "b" });

            act.Should().NotThrow();
            subject.AllowedValues.Should().BeEquivalentTo(new[] { "b", "a" });
        }

        [Fact]
        public void LimitAcceptedValues_throws_arg_null_exc_when_collection_is_null()
        {
            var subject = new ColumnConfig<TestObject>(c => c.AString);
            Action act = () => subject.LimitAcceptedValuesTo(null as IEnumerable<string>);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LimitAcceptedValues_throws_arg_exc_when_collection_is_of_wrong_type()
        {
            var subject = new ColumnConfig<TestObject>(c => c.AString);
            Action act = () => subject.LimitAcceptedValuesTo(new[] { 1, 2, 3 });

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void LimitAcceptedValues_throws_arg_exc_when_collection_is_empty()
        {
            var subject = new ColumnConfig<TestObject>(c => c.AString);
            Action act = () => subject.LimitAcceptedValuesTo(Array.Empty<string>());

            act.Should().Throw<ArgumentException>();
        }
    }
}