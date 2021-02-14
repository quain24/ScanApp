#nullable enable

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Infrastructure.Services
{
    public class ScopedMediator : IScopedMediator
    {
        private const string ExcText = "Could not create new mediator instance from scope";
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMediator _mediator;

        public ScopedMediator(IServiceScopeFactory scopeFactory, IMediator mediator)
        {
            _scopeFactory = scopeFactory;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator), "IMediator interface has not been injected!");
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return _mediator.Send(request, cancellationToken);
        }

        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            return _mediator.Send(request, cancellationToken);
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            return _mediator.Publish(notification, cancellationToken);
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            return _mediator.Publish(notification, cancellationToken);
        }

        public async Task<TResponse> SendScoped<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var scopedMediator = scope.ServiceProvider.GetService<IMediator>() ?? throw new ArgumentNullException(nameof(_scopeFactory), ExcText);
            return await scopedMediator.Send(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<object?> SendScoped(object request, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var scopedMediator = scope.ServiceProvider.GetService<IMediator>() ?? throw new ArgumentNullException(nameof(_scopeFactory), ExcText);
            return await scopedMediator.Send(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task PublishScoped(object notification, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetService<IMediator>() ?? throw new ArgumentNullException(nameof(_scopeFactory), ExcText);
            await mediator.Publish(notification, cancellationToken).ConfigureAwait(false);
        }

        public async Task PublishScoped<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetService<IMediator>() ?? throw new ArgumentNullException(nameof(_scopeFactory), ExcText);
            await mediator.Publish(notification, cancellationToken).ConfigureAwait(false);
        }
    }
}