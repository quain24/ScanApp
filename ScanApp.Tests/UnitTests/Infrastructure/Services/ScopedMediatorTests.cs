using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Infrastructure.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using ScanApp.Application.Common.Helpers.Result;
using Xunit;

namespace ScanApp.Tests.UnitTests.Infrastructure.Services
{
    public class ScopedMediatorTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var mediatorMock = new Mock<IMediator>();

            var subject = new ScopedMediator(scopeFactoryMock.Object, mediatorMock.Object);

            subject.Should().BeOfType<ScopedMediator>()
                .And.Subject.Should().BeAssignableTo<IScopedMediator>();
        }

        [Fact]
        public void Will_throw_arg_null_if_no_scope_factory_is_provided()
        {
            var mediatorMock = new Mock<IMediator>();

            Action act = () => new ScopedMediator(null, mediatorMock.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Will_throw_arg_null_if_mediator_is_provided()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();

            Action act = () => new ScopedMediator(scopeFactoryMock.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Will_send_given_request()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var mediatorMock = new Mock<IMediator>();
            var request = MediatrObjectsFixture.GetBasicRequest();

            var subject = new ScopedMediator(scopeFactoryMock.Object, mediatorMock.Object);

            await subject.Send(request);

            mediatorMock.Verify(m => m.Send(It.Is<IRequest>(r => r == request), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Will_send_given_request_as_object()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var mediatorMock = new Mock<IMediator>();
            var request = new object();

            var subject = new ScopedMediator(scopeFactoryMock.Object, mediatorMock.Object);

            await subject.Send(request);

            mediatorMock.Verify(m => m.Send(It.Is<object>(r => r == request), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Will_send_given_request_scoped()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var scopeMock = new Mock<IServiceScope>();
            scopeFactoryMock.Setup(s => s.CreateScope()).Returns(scopeMock.Object);
            var mediatorMock = new Mock<IMediator>();
            scopeMock.Setup(s => s.ServiceProvider.GetService(typeof(IMediator))).Returns(mediatorMock.Object);
            var request = MediatrObjectsFixture.GetBasicRequest();

            var subject = new ScopedMediator(scopeFactoryMock.Object, mediatorMock.Object);

            await subject.SendScoped(request);

            scopeFactoryMock.Verify(m => m.CreateScope(), Times.Once);
            mediatorMock.Verify(m => m.Send(It.Is<IRequest>(r => r == request), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void SendScoped_Will_throw_arg_null_if_did_not_get_mediator_instance_from_scope()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var scopeMock = new Mock<IServiceScope>();
            scopeFactoryMock.Setup(s => s.CreateScope()).Returns(scopeMock.Object);
            var mediatorMock = new Mock<IMediator>();
            scopeMock.Setup(s => s.ServiceProvider.GetService(typeof(IMediator))).Returns(null);
            var request = MediatrObjectsFixture.GetBasicRequest();

            var subject = new ScopedMediator(scopeFactoryMock.Object, mediatorMock.Object);

            Func<Task> act = async () => await subject.SendScoped(request);

            act.Should().Throw<ArgumentNullException>();
            scopeFactoryMock.Verify(m => m.CreateScope(), Times.Once);
            mediatorMock.Verify(m => m.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void SendScoped_sending_object_Will_throw_arg_null_if_did_not_get_mediator_instance_from_scope()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var scopeMock = new Mock<IServiceScope>();
            scopeFactoryMock.Setup(s => s.CreateScope()).Returns(scopeMock.Object);
            var mediatorMock = new Mock<IMediator>();
            scopeMock.Setup(s => s.ServiceProvider.GetService(typeof(IMediator))).Returns(null);
            var request = new object();

            var subject = new ScopedMediator(scopeFactoryMock.Object, mediatorMock.Object);

            Func<Task> act = async () => await subject.SendScoped(request);

            act.Should().Throw<ArgumentNullException>();
            scopeFactoryMock.Verify(m => m.CreateScope(), Times.Once);
            mediatorMock.Verify(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Will_send_given_request_scoped_as_object()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var scopeMock = new Mock<IServiceScope>();
            scopeFactoryMock.Setup(s => s.CreateScope()).Returns(scopeMock.Object);
            var mediatorMock = new Mock<IMediator>();
            scopeMock.Setup(s => s.ServiceProvider.GetService(typeof(IMediator))).Returns(mediatorMock.Object);
            var request = new object();

            var subject = new ScopedMediator(scopeFactoryMock.Object, mediatorMock.Object);

            await subject.SendScoped(request);

            scopeFactoryMock.Verify(m => m.CreateScope(), Times.Once);
            mediatorMock.Verify(m => m.Send(It.Is<object>(r => r == request), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Will_publish_given_notification()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var mediatorMock = new Mock<IMediator>();
            var request = MediatrObjectsFixture.GetBasicNotification();

            var subject = new ScopedMediator(scopeFactoryMock.Object, mediatorMock.Object);

            await subject.Publish(request);

            mediatorMock.Verify(m => m.Publish(It.Is<INotification>(r => r == request), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Will_publish_given_notification_as_object()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var mediatorMock = new Mock<IMediator>();
            var request = new object();

            var subject = new ScopedMediator(scopeFactoryMock.Object, mediatorMock.Object);

            await subject.Publish(request);

            mediatorMock.Verify(m => m.Publish(It.Is<object>(r => r == request), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Will_publish_given_notification_scoped()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var scopeMock = new Mock<IServiceScope>();
            scopeFactoryMock.Setup(s => s.CreateScope()).Returns(scopeMock.Object);
            var mediatorMock = new Mock<IMediator>();
            scopeMock.Setup(s => s.ServiceProvider.GetService(typeof(IMediator))).Returns(mediatorMock.Object);
            var request = MediatrObjectsFixture.GetBasicNotification();

            var subject = new ScopedMediator(scopeFactoryMock.Object, mediatorMock.Object);

            await subject.PublishScoped(request);

            scopeFactoryMock.Verify(m => m.CreateScope(), Times.Once);
            mediatorMock.Verify(m => m.Publish(It.Is<INotification>(r => r == request), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Will_publish_given_notification_scoped_as_object()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var scopeMock = new Mock<IServiceScope>();
            scopeFactoryMock.Setup(s => s.CreateScope()).Returns(scopeMock.Object);
            var mediatorMock = new Mock<IMediator>();
            scopeMock.Setup(s => s.ServiceProvider.GetService(typeof(IMediator))).Returns(mediatorMock.Object);
            var request = new object();

            var subject = new ScopedMediator(scopeFactoryMock.Object, mediatorMock.Object);

            await subject.PublishScoped(request);

            scopeFactoryMock.Verify(m => m.CreateScope(), Times.Once);
            mediatorMock.Verify(m => m.Publish(It.Is<object>(r => r == request), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void PublishScoped_Will_throw_arg_null_if_did_not_get_mediator_instance_from_scope()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var scopeMock = new Mock<IServiceScope>();
            scopeFactoryMock.Setup(s => s.CreateScope()).Returns(scopeMock.Object);
            var mediatorMock = new Mock<IMediator>();
            scopeMock.Setup(s => s.ServiceProvider.GetService(typeof(IMediator))).Returns(null);
            var request = MediatrObjectsFixture.GetBasicNotification();

            var subject = new ScopedMediator(scopeFactoryMock.Object, mediatorMock.Object);

            Func<Task> act = async () => await subject.PublishScoped(request);

            act.Should().Throw<ArgumentNullException>();
            scopeFactoryMock.Verify(m => m.CreateScope(), Times.Once);
            mediatorMock.Verify(m => m.Publish(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void PublishScoped_sending_object_Will_throw_arg_null_if_did_not_get_mediator_instance_from_scope()
        {
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var scopeMock = new Mock<IServiceScope>();
            scopeFactoryMock.Setup(s => s.CreateScope()).Returns(scopeMock.Object);
            var mediatorMock = new Mock<IMediator>();
            scopeMock.Setup(s => s.ServiceProvider.GetService(typeof(IMediator))).Returns(null);
            var request = new object();

            var subject = new ScopedMediator(scopeFactoryMock.Object, mediatorMock.Object);

            Func<Task> act = async () => await subject.PublishScoped(request);

            act.Should().Throw<ArgumentNullException>();
            scopeFactoryMock.Verify(m => m.CreateScope(), Times.Once);
            mediatorMock.Verify(m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}