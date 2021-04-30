using Divergic.Logging.Xunit;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ScanApp.Application.Common.Behaviors;
using ScanApp.Application.Common.Helpers.Result;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Application.Common.Behaviors
{
    public class LoggingBehaviourTests
    {
        public ITestOutputHelper Output { get; }
        public ICacheLogger<LoggingBehaviour<BehaviourTestFixtures.SimpleRequest, Result<string>>> Logger;

        public LoggingBehaviourTests(ITestOutputHelper output)
        {
            Output = output;
            Logger = Output.BuildLoggerFor<LoggingBehaviour<BehaviourTestFixtures.SimpleRequest, Result<string>>>();
        }

        [Fact]
        public void Creates_instance()
        {
            var accessorMock = new Mock<IHttpContextAccessor>();
            var subject = new LoggingBehaviour<BehaviourTestFixtures.SimpleRequest, Result<string>>(Logger, accessorMock.Object);

            subject.Should().BeOfType<LoggingBehaviour<BehaviourTestFixtures.SimpleRequest, Result<string>>>()
                .And.BeAssignableTo(typeof(IPipelineBehavior<BehaviourTestFixtures.SimpleRequest, Result<string>>));
        }

        [Fact]
        public void Creates_instance_without_httpAccessor()
        {
            var subject = new LoggingBehaviour<BehaviourTestFixtures.SimpleRequest, Result<string>>(Logger, null);

            subject.Should().BeOfType<LoggingBehaviour<BehaviourTestFixtures.SimpleRequest, Result<string>>>()
                .And.BeAssignableTo(typeof(IPipelineBehavior<BehaviourTestFixtures.SimpleRequest, Result<string>>));
        }

        [Fact]
        public void Throws_arg_null_exc_if_logger_is_missing()
        {
            var accessorMock = new Mock<IHttpContextAccessor>();
            Action act = () => _ = new LoggingBehaviour<BehaviourTestFixtures.SimpleRequest, Result<string>>(null, accessorMock.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Will_log_proper_request()
        {
            var request = new BehaviourTestFixtures.SimpleRequest();
            var serializedRequest = JsonSerializer.Serialize(request);
            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(m => m.HttpContext.User.Identity.Name).Returns("name");
            var requestDelegateMock = new Mock<RequestHandlerDelegate<Result<string>>>();
            requestDelegateMock.Setup(m => m.Invoke()).ReturnsAsync(new Result<string>().SetOutput("valid"));
            var subject = new LoggingBehaviour<BehaviourTestFixtures.SimpleRequest, Result<string>>(Logger, accessorMock.Object);

            var result = await subject.Handle(request, CancellationToken.None, requestDelegateMock.Object);

            Logger.Entries.Should().HaveCount(3);
            Logger.Entries.Should().ContainSingle(f =>
                f.LogLevel == LogLevel.Information && f.Message.Contains("[START] [name]") &&
                f.Message.Contains(nameof(BehaviourTestFixtures.SimpleRequest)));
            Logger.Entries.Should().ContainSingle(f =>
                f.LogLevel == LogLevel.Information && f.Message.Contains("[DATA] [name]") &&
                f.Message.Contains(nameof(BehaviourTestFixtures.SimpleRequest)) &&
                f.Message.Contains(serializedRequest));
            Logger.Entries.Should().ContainSingle(f =>
                f.LogLevel == LogLevel.Information && f.Message.Contains("[FINISHED] [name]") &&
                f.Message.Contains(nameof(BehaviourTestFixtures.SimpleRequest)) &&
                f.Message.Contains(result.GetType().Name));
        }

        [Fact]
        public async Task Will_pass_through_result_generated_by_handler()
        {
            var request = new BehaviourTestFixtures.SimpleRequest();
            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(m => m.HttpContext.User.Identity.Name).Returns("name");
            var requestDelegateMock = new Mock<RequestHandlerDelegate<Result<string>>>();
            requestDelegateMock.Setup(m => m.Invoke()).ReturnsAsync(new Result<string>().SetOutput("valid"));
            var subject = new LoggingBehaviour<BehaviourTestFixtures.SimpleRequest, Result<string>>(Logger, accessorMock.Object);

            var result = await subject.Handle(request, CancellationToken.None, requestDelegateMock.Object);

            result.Should().BeEquivalentTo(new Result<string>().SetOutput("valid"));
        }

        [Fact]
        public async Task Will_function_without_IHttpContextAccessor()
        {
            var request = new BehaviourTestFixtures.SimpleRequest();
            var serializedRequest = JsonSerializer.Serialize(request);
            var requestDelegateMock = new Mock<RequestHandlerDelegate<Result<string>>>();
            requestDelegateMock.Setup(m => m.Invoke()).ReturnsAsync(new Result<string>().SetOutput("valid"));
            var subject = new LoggingBehaviour<BehaviourTestFixtures.SimpleRequest, Result<string>>(Logger, null);

            var result = await subject.Handle(request, CancellationToken.None, requestDelegateMock.Object);

            Logger.Entries.Should().HaveCount(3);
            Logger.Entries.Should().ContainSingle(f =>
                f.LogLevel == LogLevel.Information && f.Message.Contains("[START] [Unknown]") &&
                f.Message.Contains(nameof(BehaviourTestFixtures.SimpleRequest)));
            Logger.Entries.Should().ContainSingle(f =>
                f.LogLevel == LogLevel.Information && f.Message.Contains("[DATA] [Unknown]") &&
                f.Message.Contains(nameof(BehaviourTestFixtures.SimpleRequest)) &&
                f.Message.Contains(serializedRequest));
            Logger.Entries.Should().ContainSingle(f =>
                f.LogLevel == LogLevel.Information && f.Message.Contains("[FINISHED] [Unknown]") &&
                f.Message.Contains(nameof(BehaviourTestFixtures.SimpleRequest)) &&
                f.Message.Contains(result.GetType().Name));
        }

        [Fact]
        public async Task Will_not_fail_and_inform_if_cannot_serialize_data_in_request()
        {
            var request = new BehaviourTestFixtures.NonSerializableRequest();
            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(m => m.HttpContext.User.Identity.Name).Returns("name");
            var requestDelegateMock = new Mock<RequestHandlerDelegate<Result<string>>>();
            var logger = Output.BuildLoggerFor<LoggingBehaviour<BehaviourTestFixtures.NonSerializableRequest, Result<string>>>();

            requestDelegateMock.Setup(m => m.Invoke()).ReturnsAsync(new Result<string>().SetOutput("valid"));
            var subject = new LoggingBehaviour<BehaviourTestFixtures.NonSerializableRequest, Result<string>>(logger, accessorMock.Object);

            var result = await subject.Handle(request, CancellationToken.None, requestDelegateMock.Object);

            logger.Entries.Should().HaveCount(3);
            logger.Entries.Should().ContainSingle(f =>
                f.LogLevel == LogLevel.Information && f.Message.Contains("[START] [name]") &&
                f.Message.Contains(nameof(BehaviourTestFixtures.NonSerializableRequest)));
            logger.Entries.Should().ContainSingle(f =>
                f.LogLevel == LogLevel.Information && f.Message.Contains("[Serialization ERROR] [name]") &&
                f.Message.Contains(nameof(BehaviourTestFixtures.NonSerializableRequest)) &&
                f.Message.Contains(" Could not serialize the request data."));
            logger.Entries.Should().ContainSingle(f =>
                f.LogLevel == LogLevel.Information && f.Message.Contains("[FINISHED] [name]") &&
                f.Message.Contains(nameof(BehaviourTestFixtures.NonSerializableRequest)) &&
                f.Message.Contains(result.GetType().Name));
        }

        [Fact]
        public async Task Reports_conclusion_and_code_if_response_is_of_type_result_and_valid()
        {
            var request = new BehaviourTestFixtures.NonSerializableRequest();
            var requestDelegateMock = new Mock<RequestHandlerDelegate<Result<string>>>();
            var logger = Output.BuildLoggerFor<LoggingBehaviour<BehaviourTestFixtures.NonSerializableRequest, Result<string>>>();

            requestDelegateMock.Setup(m => m.Invoke()).ReturnsAsync(new Result<string>().SetOutput("valid"));
            var subject = new LoggingBehaviour<BehaviourTestFixtures.NonSerializableRequest, Result<string>>(logger, null);

            var result = await subject.Handle(request, CancellationToken.None, requestDelegateMock.Object);

            var info = $"{result.Conclusion} - {result.ResultType}";
            logger.Entries.Should().ContainSingle(f =>
                f.LogLevel == LogLevel.Information && f.Message.Contains("[FINISHED] [Unknown]") &&
                f.Message.Contains(nameof(BehaviourTestFixtures.NonSerializableRequest)) &&
                f.Message.Contains(result.GetType().Name) &&
                f.Message.Contains(info));
        }

        [Fact]
        public async Task Reports_conclusion_and_error_code_if_response_is_of_type_result_and_invalid()
        {
            var request = new BehaviourTestFixtures.NonSerializableRequest();
            var requestDelegateMock = new Mock<RequestHandlerDelegate<Result<string>>>();
            var logger = Output.BuildLoggerFor<LoggingBehaviour<BehaviourTestFixtures.NonSerializableRequest, Result<string>>>();

            requestDelegateMock.Setup(m => m.Invoke()).ReturnsAsync(new Result<string>(ErrorType.Cancelled).SetOutput("invalid"));
            var subject = new LoggingBehaviour<BehaviourTestFixtures.NonSerializableRequest, Result<string>>(logger, null);

            var result = await subject.Handle(request, CancellationToken.None, requestDelegateMock.Object);

            var info = $"{result.Conclusion} - {result.ErrorDescription.ErrorType}";
            logger.Entries.Should().ContainSingle(f =>
                f.LogLevel == LogLevel.Information && f.Message.Contains("[FINISHED] [Unknown]") &&
                f.Message.Contains(nameof(BehaviourTestFixtures.NonSerializableRequest)) &&
                f.Message.Contains(result.GetType().Name) &&
                f.Message.Contains(info));
        }

        [Fact]
        public async Task Reports_type_of_request_result_if_result_type_ne_Result()
        {
            var request = new BehaviourTestFixtures.NonResultRequest();
            var requestDelegateMock = new Mock<RequestHandlerDelegate<string>>();
            var logger = Output.BuildLoggerFor<LoggingBehaviour<BehaviourTestFixtures.NonResultRequest, string>>();

            requestDelegateMock.Setup(m => m.Invoke()).ReturnsAsync("valid");
            var subject = new LoggingBehaviour<BehaviourTestFixtures.NonResultRequest, string>(logger, null);

            var result = await subject.Handle(request, CancellationToken.None, requestDelegateMock.Object);

            logger.Entries.Should().ContainSingle(f =>
                f.LogLevel == LogLevel.Information && f.Message.Contains("[FINISHED] [Unknown]") &&
                f.Message.Contains(nameof(BehaviourTestFixtures.NonResultRequest)) &&
                f.Message.Contains(result.GetType().Name));
        }
    }
}