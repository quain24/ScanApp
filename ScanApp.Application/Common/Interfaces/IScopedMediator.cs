#nullable enable
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    /// <summary>
    /// <inheritdoc cref="IMediator"/><br/>
    /// This implementation will send each <see cref="IRequest"/> in a new scope.
    /// </summary>
    public interface IScopedMediator : IMediator
    {
        /// <summary>
        /// <inheritdoc cref="ISender.Send{TResponse}"/><br/>
        /// This request will be executed in new short-lived scope
        /// </summary>
        Task<TResponse> SendScoped<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        /// <summary>
        /// <inheritdoc cref="ISender.Send{TResponse}"/><br/>
        /// This request will be executed in new short-lived scope
        /// </summary>
        Task<object?> SendScoped(object request, CancellationToken cancellationToken = default);
    }
}