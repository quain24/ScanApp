using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ScanApp.Application.Common.Helpers.Result;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Behaviors
{
    /// <summary>
    /// Logs every MediatR request made - Provide user name of user that executed this request and<br/>
    /// data included in request, if any
    /// </summary>
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private const string NoData = "{}";
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
        private readonly IHttpContextAccessor _accessor;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger, IHttpContextAccessor accessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            var message = typeof(TResponse).Name;
            if (response is Result result)
            {
                var info = result.Conclusion
                    ? result.ResultType?.ToString()
                    : result.ErrorDescription?.ToString()
                    ?? "Unknown";

                message += $" {result.Conclusion} - {info}";
            }

            _logger.LogInformation("[FINISHED] [{name}] {request} with response of type {response}", userName, requestName, message);
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