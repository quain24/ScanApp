﻿using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.ExceptionHandlers
{
    public class DBUpdateConcurrencyExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
        where TRequest : IRequest<TResponse>
        where TResponse : Result, new()
        where TException : DbUpdateConcurrencyException
    {
        public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
            var response = new TResponse();
            var name = request.GetType().Name;
            response.Set(ErrorType.ConcurrencyFailure, $"{name} - {exception.InnerException?.Message ?? exception.Message}.", exception);
            state.SetHandled(response);
            return Task.CompletedTask;
        }
    }
}