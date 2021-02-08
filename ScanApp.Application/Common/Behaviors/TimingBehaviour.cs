using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Behaviors
{
    /// <summary>
    /// Measure and log execution time of each MediatR request
    /// </summary>
    public class TimingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
        private readonly IHttpContextAccessor _accessor;

        public TimingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _accessor = accessor;
        }

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