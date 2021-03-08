using Fluxor;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScanApp.Store.Features.Middleware
{
    public class LoggingMiddleware : Fluxor.Middleware
    {
        private const string FluxorPrefix = "[FLUXOR] ";
        private const string NoData = "{}";
        private readonly string _userName;
        private readonly Action<string> _loggingInfo;
        private readonly ILogger<LoggingMiddleware> _logger;
        private IStore _store;

        public LoggingMiddleware(ILogger<LoggingMiddleware> logger, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _loggingInfo = (value => _logger?.LogInformation(FluxorPrefix + "{value}", value));
            _userName = accessor is null
                ? throw new ArgumentNullException(nameof(accessor))
                : accessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        }

        public override Task InitializeAsync(IStore store)
        {
            _store = store;
            _logger.LogInformation(FluxorPrefix + "[Initialized states] {states}", string.Join(", ", _store.Features.Values.Select(f => f.GetName()).ToArray()));
            return Task.CompletedTask;
        }

        public override void BeforeDispatch(object action)
        {
            var actionName = action?.GetType().Name ?? "Unknown type";

            _logger.LogInformation(FluxorPrefix + "[START] [{name}] {request}", _userName, actionName);

            try
            {
                if (TryGetDataFrom(action, out var data))
                    _logger.LogInformation(FluxorPrefix + "[DATA] [{name}] {requestName}: {props}", _userName, actionName, data);
            }
            catch (NotSupportedException)
            {
                _logger.LogInformation(FluxorPrefix + "[Serialization ERROR] [{name}] {requestName} - Could not serialize the request data.", _userName, actionName);
            }
        }

        public override void AfterDispatch(object action)
        {
            var actionName = action?.GetType().Name ?? "Unknown type";
            _logger.LogInformation(FluxorPrefix + "[FINISHED] [{name}] {request}", _userName, actionName);
        }

        private static bool TryGetDataFrom(object action, out string data)
        {
            data = string.Empty;
            var deserializedData = JsonSerializer.Serialize(action);
            if (deserializedData.Equals(NoData, StringComparison.OrdinalIgnoreCase))
                return false;
            data = deserializedData;
            return true;
        }
    }
}