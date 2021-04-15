using System;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Infrastructure.Identity;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Infrastructure.Identity
{
    public class IdentityResultExtensionsTests
    {
        public ITestOutputHelper Output { get; }

        public IdentityResultExtensionsTests(ITestOutputHelper output)
        {
            Output = output;
        }

        public static IEnumerable<object[]> GetFailedResults()
        {
            yield return new object[] { IdentityResult.Failed() };
            yield return new object[] { IdentityResult.Failed(new IdentityError { Code = "code", Description = "description" }) };
            yield return new object[] { IdentityResult.Failed(new IdentityError { Code = "code", Description = "description" }, new IdentityError { Code = "code_2", Description = "description_2" }) };
            yield return new object[] { IdentityResult.Failed(new IdentityError { Code = "code" }) };
        }

        [Theory]
        [MemberData(nameof(GetFailedResults))]
        public void CombineErrors_will_return_error_as_string_enumerable_in_proper_format(IdentityResult identityResult)
        {
            var result = identityResult.CombineErrors();

            var expected = new string[identityResult.Errors.Count()];
            for (var i = 0; i < expected.Length; i++)
            {
                var error = identityResult.Errors.ElementAt(i);
                expected[i] = $"{error.Code ?? "no_code"} | {error.Description ?? "no_description"}";
            }
            result.Should().BeEquivalentTo(expected);

            foreach (var message in result)
            {
                Output.WriteLine(message);
            }
        }

        [Fact]
        public void IsConcurrencyFailure_returns_true_if_concurrency_error_code_is_present()
        {
            var identityResult = IdentityResult.Failed(new IdentityError { Code = "ConcurrencyFailure" });

            identityResult.IsConcurrencyFailure().Should().BeTrue();
        }

        [Fact]
        public void IsConcurrencyFailure_returns_false_if_concurrency_error_code_is_not_present()
        {
            var identityResult = IdentityResult.Failed(new IdentityError { Code = "otherErrorCode" });

            identityResult.IsConcurrencyFailure().Should().BeFalse();
        }

        public static IEnumerable<object[]> GetResultsWithDuplicationError()
        {
            yield return new object[] { IdentityResult.Failed(new IdentityError { Code = "DuplicateName", Description = "description" }) };
            yield return new object[] { IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName", Description = "description" }, new IdentityError { Code = "code_2", Description = "description_2" }) };
            yield return new object[] { IdentityResult.Failed(new IdentityError { Code = "DuplicateRoleName" }) };
        }

        [Theory]
        [MemberData(nameof(GetResultsWithDuplicationError))]
        public void IsDuplicatedNameError_will_return_true_if_duplication_error_is_detected(IdentityResult identityResult)
        {
            identityResult.IsDuplicatedNameError().Should().BeTrue();
        }

        [Fact]
        public void IsDuplicatedNameError_returns_false_if_concurrency_error_code_is_not_present()
        {
            var identityResult = IdentityResult.Failed(new IdentityError { Code = "otherErrorCode" });

            identityResult.IsDuplicatedNameError().Should().BeFalse();
        }

        [Fact]
        public void IsAlreadyInRole_returns_true_if_already_in_role_error_code_is_present()
        {
            var identityResult = IdentityResult.Failed(new IdentityError { Code = "UserAlreadyInRole" });

            identityResult.IsAlreadyInRole().Should().BeTrue();
        }

        [Fact]
        public void IsAlreadyInRole_returns_false_if_concurrency_error_code_is_not_present()
        {
            var identityResult = IdentityResult.Failed(new IdentityError { Code = "otherErrorCode" });

            identityResult.IsAlreadyInRole().Should().BeFalse();
        }

        public static IEnumerable<object[]> GetResultsWithErrors()
        {
            yield return new object[] { IdentityResult.Failed(new IdentityError { Code = "ConcurrencyFailure", Description = "description" }), ErrorType.ConcurrencyFailure };
            yield return new object[] { IdentityResult.Failed(new IdentityError { Code = "UserAlreadyInRole" }), ErrorType.Duplicated };
            yield return new object[] { IdentityResult.Failed(new IdentityError { Code = "non_descriptive_code" }), ErrorType.NotValid };
            yield return new object[]
            {
                IdentityResult.Failed(new IdentityError {Code = "DuplicateName", Description = "description"},
                new IdentityError {Code = "code_2", Description = "description_2"}), ErrorType.Duplicated
            };
        }

        [Theory]
        [MemberData(nameof(GetResultsWithErrors))]
        public void AsResult_returns_invalid_result_with_adequate_error_type_set(IdentityResult identityResult, ErrorType expectedErrorType)
        {
            var result = identityResult.AsResult();

            result.Should().BeOfType<Result>();
            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(expectedErrorType);
            result.Output.Should().BeNull("no object has been passed as parameter");
        }

        [Fact]
        public void AsResult_returns_invalid_result_of_proper_type_and_output_object_when_provided()
        {
            object testObject = new string("test");
            var result = IdentityResult
                .Failed(new IdentityError { Code = "non_descriptive_code", Description = "description" })
                .AsResult(testObject);

            result.Should().BeOfType(typeof(Result<object>))
                .And.BeAssignableTo<Result>();
            result.Conclusion.Should().BeFalse();
            result.Output.Should().BeOfType(testObject.GetType());
        }

        [Fact]
        public void AsResult_returns_valid_result_when_identity_result_is_success()
        {
            var result = IdentityResult
                .Success
                .AsResult();

            result.Should().BeOfType<Result>();
            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeNull();
        }

        [Fact]
        public void AsResult_returns_valid_result_of_proper_type_when_identity_result_is_success_and_data_is_given()
        {
            string test = "data";
            var result = IdentityResult
                .Success
                .AsResult(test);

            result.Should().BeOfType<Result<string>>()
                .And.BeAssignableTo<Result>();
            result.Conclusion.Should().BeTrue();
            result.Output.Should().Be(test);
        }

        [Fact]
        public void AsResult_will_throw_arg_null_when_given_identity_result_is_null()
        {
            Action act = () => (null as IdentityResult).AsResult();

            act.Should().Throw<ArgumentNullException>();
        }
    }
}