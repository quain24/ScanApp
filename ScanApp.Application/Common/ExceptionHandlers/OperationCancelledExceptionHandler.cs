using MediatR;
using MediatR.Pipeline;
using ScanApp.Application.Common.Helpers.Result;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.ExceptionHandlers
{
    public class OperationCancelledExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
        where TRequest : IRequest<TResponse>
        where TResponse : Result, new()
        where TException : OperationCanceledException
    {
        public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
            var response = new TResponse();
            var name = request.GetType().Name;
            response.Set(ErrorType.Canceled, $"{name} - Request has been canceled.", exception: exception);
            state.SetHandled(response);
            return Task.CompletedTask;
        }
    }
}