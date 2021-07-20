using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Internal;
using MediatR.Pipeline;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.HesHub.Depots.Queries.AllDepots;

namespace ScanApp.Application.Common.ExceptionHandlers
{
    /// <summary>
    /// Behavior for executing all <see cref="IRequestExceptionHandler{TRequest,TResponse,TException}"/>
    ///     or <see cref="RequestExceptionHandler{TRequest,TResponse}"/> instances
    ///     after an exception is thrown by the following pipeline steps
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    public class RequestExceptionProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ServiceFactory _serviceFactory;

        public RequestExceptionProcessorBehavior(ServiceFactory serviceFactory) => _serviceFactory = serviceFactory;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {

                var t = _serviceFactory.Invoke(
                    typeof(IRequestExceptionHandler<,,>).MakeGenericType(typeof(AllDepotsQuery), typeof(Result),
                        typeof(OperationCanceledException)));
                var a = "";
            }
            catch(Exception ex)
            {
                var g = "";
            }
            try
            {
                return await next().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                var state = new RequestExceptionHandlerState<TResponse>();
                Type? exceptionType = null;

                while (!state.Handled && exceptionType != typeof(Exception))
                {
                    exceptionType = exceptionType == null ? exception.GetType() : exceptionType.BaseType;
                    var exceptionHandlers = GetExceptionHandlers(request, exceptionType, out MethodInfo handleMethod);

                    foreach (var exceptionHandler in exceptionHandlers)
                    {
                        await ((Task)handleMethod.Invoke(exceptionHandler, new object[] { request, exception, state, cancellationToken })).ConfigureAwait(false);

                        if (state.Handled)
                        {
                            break;
                        }
                    }
                }

                if (!state.Handled)
                {
                    throw;
                }

                return state.Response!; //cannot be null if Handled
            }
        }

        private IList<object> GetExceptionHandlers(TRequest request, Type exceptionType, out MethodInfo handleMethodInfo)
        {
            var exceptionHandlerInterfaceType = typeof(IRequestExceptionHandler<,,>).MakeGenericType(typeof(TRequest), typeof(TResponse), exceptionType);
            var enumerableExceptionHandlerInterfaceType = typeof(IEnumerable<>).MakeGenericType(exceptionHandlerInterfaceType);
            handleMethodInfo = exceptionHandlerInterfaceType.GetMethod(nameof(IRequestExceptionHandler<TRequest, TResponse, Exception>.Handle));

            var exceptionHandlers = (IEnumerable<object>)_serviceFactory.Invoke(enumerableExceptionHandlerInterfaceType);

            return HandlersOrderer.Prioritize(exceptionHandlers.ToList(), request);
        }
    }
}
