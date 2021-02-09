using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ScanApp.Application.Common.Helpers.Result;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Behaviors
{
    /// <summary>
    /// Will validate every request that provides a <see cref="Result"/> or <see cref="Result{T}"/> using corresponding validators
    /// </summary>
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result, new()
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehaviour<TRequest, TResponse>> _logger;
        private readonly IHttpContextAccessor _accessor;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehaviour<TRequest, TResponse>> logger, IHttpContextAccessor accessor)
        {
            _validators = validators;
            _logger = logger;
            _accessor = accessor;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!_validators.Any())
                return next();

            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count == 0)
                return next();

            var response = new TResponse();
            var errors = failures.Select(f => $"\"{f.PropertyName}\" | {f.ErrorCode} | {f.ErrorMessage}").ToArray();
            response.Set(ErrorType.NotValid, errors);

            var userName = _accessor?.HttpContext?.User?.Identity?.Name ?? "Unknown";
            _logger.LogWarning("[VALIDATION ERROR] [{name}] {request} - {errors} ", userName, typeof(TRequest).Name, response.ErrorDescription.ErrorMessage);

            return Task.FromResult(response);
        }
    }
}