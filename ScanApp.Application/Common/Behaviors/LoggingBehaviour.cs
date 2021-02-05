using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Behaviors
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private static string NoData = "{}";
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
        private readonly IHttpContextAccessor _accessor;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _accessor = accessor;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var userName = _accessor?.HttpContext?.User?.Identity?.Name ?? "Unknown";
            var requestName = typeof(TRequest).Name;
            _logger.LogInformation("[START] [{name}] {request}", userName, requestName);

            try
            {
                if (TryGetDataFrom(request, out var data))
                    _logger.LogInformation("[DATA] [{name}] {requestName}: {props}", userName, requestName, data);
            }
            catch (NotSupportedException)
            {
                _logger.LogInformation("[Serialization ERROR] [{name}] {requestName} - Could not serialize the request data.", userName, requestName);
            }

            var response = await next();

            _logger.LogInformation("[FINISHED] [{name}] {request} with response of type {response}", userName, requestName, typeof(TResponse).Name);
            return response;
        }

        private static bool TryGetDataFrom(TRequest request, out string data)
        {
            data = string.Empty;
            var deserializedData = JsonSerializer.Serialize(request);
            if (deserializedData.Equals(NoData, StringComparison.OrdinalIgnoreCase))
                return false;
            data = deserializedData;
            return true;
        }
    }
}