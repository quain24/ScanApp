using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using ScanApp.Infrastructure.Persistence;
using System;

namespace ScanApp.Tests.UnitTests.Infrastructure.Identity
{
    public class AppDbContextFactoryMockFixture
    {
        public static Mock<IDbContextFactory<ApplicationDbContext>> CreateSimpleFactoryMock(string id = null, Mock<ApplicationDbContext> contextMock = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: id ?? Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var ctxFacMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            ctxFacMock.Setup(c => c.CreateDbContext()).Returns(contextMock?.Object ?? new ApplicationDbContext(options));
            return ctxFacMock;
        }
    }
}