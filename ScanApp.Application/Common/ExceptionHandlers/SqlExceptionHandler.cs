using MediatR;
using MediatR.Pipeline;
using ScanApp.Application.Common.Helpers.Result;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.ExceptionHandlers
{
    public class SqlExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
        where TRequest : IRequest<TResponse>
        where TResponse : Result, new()
        where TException : DbException
    {
        public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
            if (exception is not SqlException exc)
                return Task.CompletedTask;

            var response = new TResponse();
            var name = request.GetType().Name;
            response.Set(ErrorType.DatabaseError, $"{name} - {exc.InnerException?.Message ?? exc.Message}.", exc);
            state.SetHandled(response);
            return Task.CompletedTask;
        }
    }
}