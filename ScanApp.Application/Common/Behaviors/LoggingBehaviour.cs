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
    /// Logs every <see cref="Mediator"/> request made - Provide name of user that executed this request and data included in request, if any.
    /// <para>Will log finished requests result and, if request result is of type <see cref="Result"/> or <see cref="Result{T}"/> - also it's result / error code and message</para>
    /// </summary>
    /// <typeparam name="TRequest">Type of handled request</typeparam>
    /// <typeparam name="TResponse">Type of response that will be returned by this behavior</typeparam>
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private const string NoData = "{}";
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// Creates new instance of <see cref="LoggingBehaviour{TRequest,TResponse}"/>
        /// </summary>
        /// <param name="logger">Logger instance that will be used to log passing requests data</param>
        /// <param name="accessor">accessor that will provide user data, such as name</param>
        /// <exception cref="ArgumentNullException">When <paramref name="logger"/> is <see langword="null"/></exception>
        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger, IHttpContextAccessor accessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accessor = accessor;
        }

        /// <summary>
        /// Handles <typeparamref name="TRequest"/> passing through mediator pipeline - logs start and end time, name of user that run the <typeparamref name="TRequest"/><br/>
        /// If possible, will try to log serialized data from the <typeparamref name="TRequest"/> as well as status /  message / error code if given <typeparamref name="TResponse"/> is of type <see cref="Result"/> or <see cref="Result{T}"/>.
        /// </summary>
        /// <remarks><paramref name="cancellationToken"/> is not used in this implementation of <see cref="IPipelineBehavior{TRequest,TResponse}"/></remarks>
        /// <param name="request">Incoming request</param>
        /// <param name="cancellationToken">(not used) A token that can be used to request cancellation of the asynchronous operation</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <returns>Awaited result of <paramref name="next"/> delegate</returns>
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