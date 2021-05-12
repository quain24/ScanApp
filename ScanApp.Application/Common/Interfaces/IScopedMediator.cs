#nullable enable

using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    /// <summary>
    /// <inheritdoc cref="IMediator"/><br/>
    /// This implementation contains methods to also send each <see cref="IRequest"/> in a new scope.
    /// </summary>
    public interface IScopedMediator : IMediator
    {
        /// <inheritdoc cref="ISender.Send{TResponse}"/>
        /// <remarks>This request will be executed in new short-lived scope.</remarks>
        Task<TResponse> SendScoped<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="ISender.Send"/>
        /// <remarks>This request will be executed in new short-lived scope.</remarks>
        Task<object?> SendScoped(object request, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="IPublisher.Publish"/>
        /// <remarks>This event will be published from new short-lived scope.</remarks>
        Task PublishScoped(object notification, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="IPublisher.Publish{TNotification}"/>
        /// <remarks>This event will be published from new short-lived scope.</remarks>
        Task PublishScoped<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification;
    }
}