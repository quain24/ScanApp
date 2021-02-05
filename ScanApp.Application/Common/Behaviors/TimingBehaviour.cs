using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Behaviors
{
    public class TimingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

        public TimingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var timer = Stopwatch.StartNew();
            var response = await next();
            timer.Stop();

            _logger.LogInformation("Handled {request} in {time} milliseconds", typeof(TRequest).Name, timer.ElapsedMilliseconds);
            return response;
        }
    }
}