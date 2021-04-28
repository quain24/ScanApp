using System;
using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Tests.UnitTests.Application.Common.Behaviors
{
    public class BehaviourTestFixtures
    {
        public record SimpleRequest(string Data = null) : IRequest<Result<string>>;

        public class NonSerializableRequest : IRequest<Result<string>>
        {
            public string Data => throw new NotSupportedException();
        }

        public record NonResultRequest(string Data = null) : IRequest<string>;

        public class SimpleHandler : IRequestHandler<SimpleRequest, Result<string>>
        {
            public Task<Result<string>> Handle(SimpleRequest request, CancellationToken cancellationToken)
            {
                var message = "valid";
                if (string.IsNullOrEmpty(request.Data) is false)
                    message += "_" + request.Data;
                return Task.FromResult(new Result<string>().SetOutput(message));
            }
        }
    }
}