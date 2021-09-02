using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ScanApp.Application.Common.Helpers.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Behaviors
{
    /// <summary>
    /// Validate every <typeparamref name="TRequest"/> passing through it using validators of type <see cref="AbstractValidator{TRequest}"/> assigned to said <typeparamref name="TRequest"/>
    /// </summary>
    /// <remarks>This class handlers validation only if <typeparamref name="TResponse"/> parameter is of type <see cref="Result"/> or <see cref="Result{T}"/> or derived</remarks>
    /// <typeparam name="TRequest">Type of handled request</typeparam>
    /// <typeparam name="TResponse">Type of response that will be returned by this behavior</typeparam>
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result, new()
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehaviour<TRequest, TResponse>> _logger;
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// Creates new instance of <see cref="ValidationBehaviour{TRequest,TResponse}"/>
        /// </summary>
        /// <param name="validators">Collection of <see cref="IValidator{TRequest}"/> assigned to particular <typeparamref name="TRequest"/></param>
        /// <param name="logger">Logger instance that will be used to log passing requests data</param>
        /// <param name="accessor">Accessor that will provide user data, such as name</param>
        /// <exception cref="ArgumentNullException">When <paramref name="logger"/> is <see langword="null"/></exception>
        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehaviour<TRequest, TResponse>> logger, IHttpContextAccessor accessor)
        {
            _validators = validators;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accessor = accessor;
        }

        /// <summary>
        /// Handles <typeparamref name="TRequest"/> passing through mediator pipeline - validates those request by using <see cref="IValidator{T}"/> collection of validators that were assigned to given <paramref name="request"/><br/>
        /// If <paramref name="request"/> was invalid, information will be logged - including all validation errors and name of user that run that request, if possible.
        /// </summary>
        /// <remarks><paramref name="cancellationToken"/> is not used in this implementation of <see cref="IPipelineBehavior{TRequest,TResponse}"/><br/>
        /// This <see cref="IPipelineBehavior{TRequest,TResponse}"/> is final - if validation fails, new <typeparamref name="TResponse"/> is returned and pipeline is stopped
        /// </remarks>
        /// <param name="request">Incoming request</param>
        /// <param name="cancellationToken">(not used) A token that can be used to request cancellation of the asynchronous operation</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <returns>
        /// <para>Awaitable task returning the <typeparamref name="TResponse"/> if <paramref name="request"/> was valid</para>
        /// <para>Awaitable task returning new <typeparamref name="TResponse"/> containing error code and combined list of validation errors if <paramref name="request"/> was invalid</para>
        /// </returns>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!_validators?.Any() ?? true)
                return await next().ConfigureAwait(false);

            var failures = (await Task.WhenAll
                (
                _validators
                    .Select(v => v.ValidateAsync(request, cancellationToken))
                )
                .ConfigureAwait(false))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count == 0)
                return await next().ConfigureAwait(false);

            var response = new TResponse();
            var errors = failures.Select(f => f.ErrorMessage + (f.ErrorCode is null ? string.Empty : $" - {f.ErrorCode}")).ToArray();
            response.Set(ErrorType.NotValid, errors);

            var userName = _accessor?.HttpContext?.User?.Identity?.Name ?? "Unknown";
            _logger.LogWarning("[VALIDATION ERROR] [{name}] {request} - {errors} ", userName, typeof(TRequest).Name, response.ErrorDescription.ErrorMessage);

            return response;
        }
    }
}