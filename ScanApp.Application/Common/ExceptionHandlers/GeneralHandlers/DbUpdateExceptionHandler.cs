using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.ExceptionHandlers.GeneralHandlers
{
    public class DbUpdateExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
        where TRequest : IRequest<TResponse>
        where TResponse : Result, new()
        where TException : DbUpdateException
    {
        public virtual Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
            var response = new TResponse();
            var name = request.GetType().Name;
            response.Set(ErrorType.DatabaseError, $"{name} - {exception.InnerException?.Message ?? exception.Message}", exception);
            state.SetHandled(response);
            return Task.CompletedTask;
        }
    }
}