using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.ExceptionHandlers.GeneralHandlers;
using ScanApp.Application.Common.Helpers.Result;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.ExceptionHandlers
{
    public class DbUpdateConcurrencyExceptionHandler<TRequest, TResponse, TException> : DbUpdateExceptionHandler<TRequest, TResponse, TException>
        where TRequest : IRequest<TResponse>
        where TResponse : Result, new()
        where TException : DbUpdateConcurrencyException
    {
        public override Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
            var response = new TResponse();
            var requestName = request.GetType().Name;
            var entityNames = string.Join(", ", exception.Entries.Select(x => x.Metadata.Name));
            var message = new StringBuilder(requestName).Append(" - ")
                .Append(entityNames).Append(" - ")
                .Append(exception.Message);
            if (exception.InnerException is not null)
            {
                message.Append(",\r\n").Append(exception.InnerException.Message);
            }
            response.Set(ErrorType.ConcurrencyFailure, message.ToString(), exception: exception);
            state.SetHandled(response);
            return Task.CompletedTask;
        }
    }
}