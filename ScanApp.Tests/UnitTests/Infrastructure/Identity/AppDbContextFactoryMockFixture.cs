using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Infrastructure.Persistence;
using System;

namespace ScanApp.Tests.UnitTests.Infrastructure.Identity
{
    public class AppDbContextFactoryMockFixture
    {
        public static Mock<IDbContextFactory<ApplicationDbContext>> CreateSimpleFactoryMock(string id = null, Mock<ApplicationDbContext> contextMock = null)
        {
            var ctxFacMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            ctxFacMock.Setup(c => c.CreateDbContext()).Returns(contextMock?.Object ?? new ApplicationDbContext(BuildOptions(id)));
            return ctxFacMock;
        }

        public static Mock<IContextFactory> CreateSimpleIContextFactoryMock(string id = null, Mock<IApplicationDbContext> contextMock = null)
        {
            var ctxFacMock = new Mock<IContextFactory>();
            ctxFacMock.Setup(c => c.CreateDbContext()).Returns(contextMock?.Object ?? new ApplicationDbContext(BuildOptions(id)));
            return ctxFacMock;
        }

        private static DbContextOptions<ApplicationDbContext> BuildOptions(string id)
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: id ?? Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
        }
    }
}