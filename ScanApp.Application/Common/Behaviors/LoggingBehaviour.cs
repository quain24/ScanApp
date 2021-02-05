using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Behaviors
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            //Request
            _logger.LogInformation("Handling {request}", typeof(TRequest).Name);
            var myType = request.GetType();
            var props = new List<PropertyInfo>(myType.GetProperties());
            foreach (var prop in props)
            {
                object propValue = prop.GetValue(request, null);
                _logger.LogInformation("{Property} : {@Value}", prop.Name, propValue);
            }
            var response = await next();
            //Response
            _logger.LogInformation("Handled {request} with response of {@response}", typeof(TRequest).Name, typeof(TResponse).Name);
            return response;
        }
    }
}