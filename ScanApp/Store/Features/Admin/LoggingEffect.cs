using Fluxor;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ScanApp.Store.Features.Admin
{
    public abstract class LoggingEffect<T> : Effect<T>
    {
        private readonly string _name;
        private readonly ILogger _logger;

        protected LoggingEffect(string name, ILogger logger)
        {
            _name = name;
            _logger = logger;
        }

        public abstract Task LoggingHandleAsync(T action, IDispatcher dispatcher);

        public override async Task HandleAsync(T action, IDispatcher dispatcher)
        {
            var timer = new Stopwatch();
            _logger.LogInformation("[FLUXOR EFFECT] [{name}] Start time measure...", _name);
            timer.Start();
            await LoggingHandleAsync(action, dispatcher).ConfigureAwait(false);
            timer.Stop();
            _logger.LogInformation("[FLUXOR EFFECT] [{name}] elapsed time: {time} ms", _name, timer.Elapsed.Milliseconds);
        }
    }
}