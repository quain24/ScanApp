using MediatR;
using MediatR.Pipeline;
using ScanApp.Application.Common.Helpers.Result;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace ScanApp.Application.Common.ExceptionHandlers
{
    public class OperationCancelledHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
        where TRequest : IRequest<TResponse>
        where TResponse : Result, new()
        where TException : OperationCanceledException
    {
        //public async Task Handle(TRequest request, OperationCanceledException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
        //{
            
        //}

        //public async Task Handle(TRequest request, Exception exception, RequestExceptionHandlerState<TResponse> state,
        //    CancellationToken cancellationToken)
        //{
        //    Result r = new Result(ErrorType.Cancelled);
        //    state.SetHandled(r as TResponse);
        //}
        public async Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
            var r = new TResponse();
            r.Set(ErrorType.Cancelled, "");
            
            state.SetHandled(new TResponse().SetOutput(ErrorType.Cancelled) as TResponse);
        }
    }
}