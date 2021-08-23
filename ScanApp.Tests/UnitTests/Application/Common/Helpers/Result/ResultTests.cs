using FluentAssertions;
using ScanApp.Application.Common.Helpers.Result;
using System;
using FluentAssertions.Execution;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Application.Common.Helpers.Result
{
    public class ResultTests
    {
        public ITestOutputHelper Output { get; }

        public ResultTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Creates_valid_result_instance_by_default()
        {
            var result = new ScanApp.Application.Common.Helpers.Result.Result();

            result.Conclusion.Should().BeTrue();
            result.ResultType.Should().Be(ResultType.Ok);
        }

        [Fact]
        public void Creates_valid_result_of_T_instance_by_default()
        {
            var result = new Result<string>();

            result.Conclusion.Should().BeTrue();
            result.ResultType.Should().Be(ResultType.Ok);
            result.Should().BeOfType<Result<string>>();
        }

        [Fact]
        public void Valid_result_does_not_have_ErrorDescription()
        {
            var result = new ScanApp.Application.Common.Helpers.Result.Result();

            result.ErrorDescription.Should().BeNull();
        }

        [Fact]
        public void Valid_result_of_T_does_not_have_ErrorDescription()
        {
            var result = new Result<string>();

            result.ErrorDescription.Should().BeNull();
        }

        [Fact]
        public void Invalid_result_does_not_have_ResultType()
        {
            var result = new ScanApp.Application.Common.Helpers.Result.Result(ErrorType.Canceled);

            result.ResultType.Should().BeNull();
        }

        [Fact]
        public void Invalid_result_of_T_does_not_have_ResultType()
        {
            var result = new Result<string>(ErrorType.Canceled);

            result.ResultType.Should().BeNull();
        }

        [Theory]
        [InlineData(ResultType.Ok)]
        [InlineData(ResultType.Created)]
        [InlineData(ResultType.Deleted)]
        [InlineData(ResultType.Updated)]
        public void Created_with_ResultType_will_be_valid_and_will_contain_given_type(ResultType type)
        {
            var result = new ScanApp.Application.Common.Helpers.Result.Result(type);

            result.Conclusion.Should().BeTrue();
            result.ResultType.Should().Be(type);
        }

        [Theory]
        [InlineData(ResultType.Ok)]
        [InlineData(ResultType.Created)]
        [InlineData(ResultType.Deleted)]
        [InlineData(ResultType.Updated)]
        public void Created_generic_with_ResultType_will_be_valid_and_will_contain_given_type(ResultType type)
        {
            var result = new Result<int>(type);

            result.Conclusion.Should().BeTrue();
            result.ResultType.Should().Be(type);
        }

        [Theory]
        [InlineData(ErrorType.Canceled)]
        [InlineData(ErrorType.ConfigurationError)]
        [InlineData(ErrorType.Duplicated)]
        [InlineData(ErrorType.NotFound)]
        public void Created_with_ErrorType_will_be_invalid_and_will_contain_given_error_type(ErrorType type)
        {
            var result = new ScanApp.Application.Common.Helpers.Result.Result(type);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.Should().BeOfType<ErrorDescription>()
                .And.Subject.As<ErrorDescription>().ErrorType.Should().Be(type);
        }

        [Theory]
        [InlineData(ErrorType.Canceled)]
        [InlineData(ErrorType.ConfigurationError)]
        [InlineData(ErrorType.Duplicated)]
        [InlineData(ErrorType.NotFound)]
        public void Created_generic_with_ErrorType_will_be_invalid_and_will_contain_given_error_type(ErrorType type)
        {
            var result = new Result<long>(type);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.Should().BeOfType<ErrorDescription>()
                .And.Subject.As<ErrorDescription>().ErrorType.Should().Be(type);
        }

        [Theory]
        [InlineData(ErrorType.Canceled, "TestCode")]
        [InlineData(ErrorType.ConfigurationError, "00458")]
        [InlineData(ErrorType.Duplicated, "TestCode")]
        [InlineData(ErrorType.NotFound, "@@AAll")]
        public void Created_with_ErrorType_and_error_code_will_be_invalid_and_will_contain_given_error_type_and_code(ErrorType type, string code)
        {
            var result = new ScanApp.Application.Common.Helpers.Result.Result(type, string.Empty, code);

            using var scope = new AssertionScope();
            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.Should().BeOfType<ErrorDescription>()
                .And.Subject.As<ErrorDescription>().ErrorType.Should().Be(type);
            result.ErrorDescription.ErrorCode.Should().Be(code);
        }

        [Theory]
        [InlineData(ErrorType.Canceled, "TestCode")]
        [InlineData(ErrorType.ConfigurationError, "00458")]
        [InlineData(ErrorType.Duplicated, "TestCode")]
        [InlineData(ErrorType.NotFound, "@@AAll")]
        public void Created_generic_with_ErrorType_and_error_code_will_be_invalid_and_will_contain_given_error_type_and_code(ErrorType type, string code)
        {
            var result = new Result<long>(type, string.Empty, code);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.Should().BeOfType<ErrorDescription>()
                .And.Subject.As<ErrorDescription>().ErrorType.Should().Be(type);
            result.ErrorDescription.ErrorCode.Should().Be(code);
        }

        [Fact]
        public void Given_error_type_and_exception_will_store_exception_and_provide_basic_data_directly()
        {
            var exception = new ArgumentNullException("name", "message");

            var result = new ScanApp.Application.Common.Helpers.Result.Result(ErrorType.Canceled, exception);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorMessage.Should().Be(exception.Message);
            result.ErrorDescription.Exception.Should().BeOfType<ArgumentNullException>();
            result.ErrorDescription.StackTrace.Should().Be(exception.StackTrace);
        }

        [Fact]
        public void Given_error_type_and_exception_to_generic_result_it_will_store_exception_and_provide_basic_data_directly()
        {
            var exception = new ArgumentNullException("name", "message");

            var result = new Result<string>(ErrorType.Canceled, exception);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorMessage.Should().Be(exception.Message);
            result.ErrorDescription.Exception.Should().BeOfType<ArgumentNullException>();
            result.ErrorDescription.StackTrace.Should().Be(exception.StackTrace);
        }

        [Fact]
        public void Given_error_type_and_message_will_store_and_provide_message()
        {
            var result = new ScanApp.Application.Common.Helpers.Result.Result(ErrorType.Canceled, "message");

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorMessage.Should().Be("message");
        }

        [Fact]
        public void Given_error_type_and_message_to_generic_version_will_store_and_provide_message()
        {
            var result = new Result<TheoryData>(ErrorType.Canceled, "message");

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorMessage.Should().Be("message");
        }

        [Fact]
        public void Given_error_type_and_exception_and_message_a_message_property_will_hold_text_from_errorMessage()
        {
            var exception = new ArgumentNullException("name", "message");

            var result = new ScanApp.Application.Common.Helpers.Result.Result(ErrorType.Canceled, "error_message", exception: exception);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorMessage.Should().Be("error_message");
            result.ErrorDescription.Exception.Should().BeOfType<ArgumentNullException>();
            result.ErrorDescription.StackTrace.Should().Be(exception.StackTrace);
        }

        [Fact]
        public void Given_error_type_and_exception_and_message_to_generic_a_message_property_will_hold_text_from_errorMessage()
        {
            var exception = new ArgumentNullException("name", "message");

            var result = new Result<string>(ErrorType.Canceled, "error_message", exception: exception);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorMessage.Should().Be("error_message");
            result.ErrorDescription.Exception.Should().BeOfType<ArgumentNullException>();
            result.ErrorDescription.StackTrace.Should().Be(exception.StackTrace);
        }

        [Fact]
        public void Given_list_of_error_messages_will_hold_string_composed_of_them_separated_by_new_line()
        {
            var messages = new[]
            {
                "msg_a",
                "msg_b",
                "msg_c"
            };
            var expectedMessageOutput = string.Join("\n", messages);
            var result = new ScanApp.Application.Common.Helpers.Result.Result(ErrorType.Canceled, messages);

            Output.WriteLine(result.ErrorDescription.ErrorMessage);
            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorMessage.Should().Be(expectedMessageOutput);
        }

        [Fact]
        public void SetOutput_stores_output_in_result_as_object()
        {
            var itemToStore = new string("item");
            var subject = new ScanApp.Application.Common.Helpers.Result.Result();

            subject.SetOutput(itemToStore);

            (subject.Output is object).Should().BeTrue();
            subject.Output.Should().BeOfType<string>()
                .And.Subject.As<string>().Should().Be("item");
        }

        [Fact]
        public void SetOutput_stores_output_in_generic_result_as_generic_param()
        {
            var itemToStore = new string("item");
            var subject = new Result<string>().SetOutput(itemToStore);

            subject.SetOutput(itemToStore);

            subject.Should().BeOfType<Result<string>>();
            subject.Output.Should().BeOfType<string>()
                .And.Subject.Should().Be("item");
        }

        [Fact]
        public void Set_will_replace_existing_ErrorDescription()
        {
            var subject = new ScanApp.Application.Common.Helpers.Result.Result(ErrorType.Canceled, "err_message", exception: new Exception());

            subject.Set(ErrorType.Duplicated, "new_err_message");

            subject.ErrorDescription.ErrorType.Should().Be(ErrorType.Duplicated);
            subject.ErrorDescription.ErrorMessage.Should().Be("new_err_message");
            subject.ErrorDescription.Exception.Should().BeNull();
        }

        [Fact]
        public void Set_will_replace_existing_ErrorDescription_in_generic_result()
        {
            var subject = new Result<int>(ErrorType.Canceled, "err_message", exception: new Exception());

            subject.Set(ErrorType.Duplicated, "new_err_message");

            subject.Should().BeOfType<Result<int>>();
            subject.ErrorDescription.ErrorType.Should().Be(ErrorType.Duplicated);
            subject.ErrorDescription.ErrorMessage.Should().Be("new_err_message");
            subject.ErrorDescription.Exception.Should().BeNull();
        }
    }
}