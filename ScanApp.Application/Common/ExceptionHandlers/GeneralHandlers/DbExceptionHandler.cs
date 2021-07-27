using MediatR;
using MediatR.Pipeline;
using ScanApp.Application.Common.Helpers.Result;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.ExceptionHandlers.GeneralHandlers
{
    public class DbExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
        where TRequest : IRequest<TResponse>
        where TResponse : Result, new()
        where TException : DbException
    {
        public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
            var response = new TResponse();
            var name = request.GetType().Name;

            if (exception is SqlException exc)
            {
                var errors = string.Join("\r\n", exc.Errors.Cast<SqlError>().Select(x => x.Message));
                response.Set(ErrorType.DatabaseError, $"{name} - {exc.Number} - {exc.Message}\n\r{errors}", exc);
            }
            else
            {
                response.Set(ErrorType.DatabaseError, $"{name} - {exception.Message}\n\r{exception?.InnerException?.Message}", exception);
            }
            state.SetHandled(response);
            return Task.CompletedTask;
        }
    }
}