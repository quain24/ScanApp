using Moq;
using ScanApp.Application.Common.Interfaces;

namespace ScanApp.Tests.UnitTests.Application.Admin.Queries
{
    public class IContextFactoryMockFixtures
    {
        public Mock<IContextFactory> ContextFactoryMock { get; }
        public Mock<IApplicationDbContext> ContextMock { get; }

        public IContextFactoryMockFixtures(Mock<IApplicationDbContext> premadeContextMock = null)
        {
            ContextFactoryMock = new Mock<IContextFactory>();
            ContextMock = premadeContextMock ?? new Mock<IApplicationDbContext>();

            ContextFactoryMock.Setup(m => m.CreateDbContext()).Returns(ContextMock.Object);
        }
    }
}