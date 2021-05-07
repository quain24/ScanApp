using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Behaviors
{
    /// <summary>
    /// Measures and log execution time of each <see cref="Mediator"/> request made and passed through it.
    /// </summary>
    /// <typeparam name="TRequest">Type of handled request</typeparam>
    /// <typeparam name="TResponse">Type of response that will be returned by this behavior</typeparam>
    public class TimingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// Creates new instance of <see cref="TimingBehaviour{TRequest,TResponse}"/>
        /// </summary>
        /// <param name="logger">Logger instance that will be used to log passing requests data</param>
        /// <param name="accessor">accessor that will provide user data, such as name of user</param>
        /// <exception cref="ArgumentNullException">When <paramref name="logger"/> is <see langword="null"/></exception>
        public TimingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger, IHttpContextAccessor accessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accessor = accessor;
        }

        /// <summary>
        /// Handles <typeparamref name="TRequest"/> passing through mediator pipeline - logs execution time and name of user that run the <typeparamref name="TRequest"/><br/>
        /// </summary>
        /// <remarks><paramref name="cancellationToken"/> is not used in this implementation of <see cref="IPipelineBehavior{TRequest,TResponse}"/></remarks>
        /// <param name="request">Incoming request</param>
        /// <param name="cancellationToken">(not used) A token that can be used to request cancellation of the asynchronous operation</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <returns>Awaitable task returning the <typeparamref name="TResponse"/></returns>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var timer = Stopwatch.StartNew();
            var userName = _accessor?.HttpContext?.User?.Identity?.Name ?? "Unknown";
            TResponse response;

            try
            {
                response = await next();
            }
            finally
            {
                timer.Stop();
                _logger.LogInformation("[EXECUTION TIME] [{name}] {request} {time} milliseconds", userName, typeof(TRequest).Name, timer.ElapsedMilliseconds);
            }

            return response;
        }
    }
}