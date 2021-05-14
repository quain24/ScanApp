using MediatR;

namespace ScanApp.Tests.UnitTests.Infrastructure.Services
{
    public class MediatrObjectsFixture
    {
        public static IRequest GetBasicRequest() => new Request();

        public static INotification GetBasicNotification() => new Notification();

        public class Request : IRequest
        {
        }

        public class Notification : INotification
        {
        }
    }
}