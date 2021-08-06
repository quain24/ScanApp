using EntityFramework.Exceptions.Common;
using MediatR;
using MediatR.Pipeline;
using ScanApp.Application.Common.ExceptionHandlers.GeneralHandlers;
using ScanApp.Application.Common.Helpers.Result;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.ExceptionHandlers
{
    public class UniqueConstraintExceptionHandler<TRequest, TResponse, TException> : DbUpdateExceptionHandler<TRequest, TResponse, TException>
        where TRequest : IRequest<TResponse>
        where TResponse : Result, new()
        where TException : UniqueConstraintException
    {
        public override Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
        {
            var response = new TResponse();
            var requestName = request.GetType().Name;
            var entityNames = string.Join(", ", exception.Entries.Select(x => x.Metadata.Name));
            var message = new StringBuilder(requestName).Append(" - ")
                .Append(entityNames).Append(": ")
                .Append(exception.InnerException?.Message);

            response.Set(ErrorType.UniqueConstraintViolation, message.ToString(), exception);
            state.SetHandled(response);
            return Task.CompletedTask;
        }
    }
}