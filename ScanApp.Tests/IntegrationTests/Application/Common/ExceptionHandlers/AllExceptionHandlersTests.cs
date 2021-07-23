using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Helpers.Result;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.IntegrationTests.Application.Common.ExceptionHandlers
{
    public class AllExceptionHandlersTests : ServiceProviderWithMediatrFixture
    {
        private ITestOutputHelper Output { get; }

        public AllExceptionHandlersTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Theory]
        [ClassData(typeof(ExceptionHandlerFixtures.HandledExceptions))]
        public async Task Handlers_will_intercept_and_handle_correct_exceptions(IRequest<Result> command, Type exceptionType)
        {
            var result = await Provider.GetRequiredService<IMediator>().Send(command);

            using var scope = new AssertionScope();
            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.Exception.Should().BeOfType(exceptionType);

            Output.WriteLine("Command type: " + command?.GetType()?.FullName);
            Output.WriteLine("Expected Exception type: " + exceptionType?.FullName);
            Output.WriteLine("Result Exception type: " + result?.ErrorDescription?.Exception?.GetType()?.FullName);
            Output.WriteLine("Result Error type: " + result?.ErrorDescription?.ErrorType);
        }

        [Theory]
        [ClassData(typeof(ExceptionHandlerFixtures.NotHandledExceptions))]
        public async Task Unhandled_exceptions_will_be_passed_through_and_thrown(IRequest<Result> command)
        {
            Func<Task> act = async () => await Provider.GetRequiredService<IMediator>().Send(command);
            Output.WriteLine($"Exception type: {(command as ExceptionHandlerFixtures.Command).ExceptionType}");
            await act.Should().ThrowAsync<Exception>("without handler exception should bubble up.");
        }

        [Fact]
        public void All_handlers_are_tested()
        {
            var manuallyProvidedTypes = ExceptionHandlerFixtures.ManuallyProvidedHandledExceptionTypes.Select(x => x.Type).ToList()
                .OrderBy(x => x.Name);
            var detectedHandlers = ExceptionHandlerFixtures.DetectedHandledTypes().OrderBy(x => x.Name);

            manuallyProvidedTypes.Should().BeEquivalentTo(detectedHandlers);
        }
    }
}