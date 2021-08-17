using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ScanApp.Tests.UnitTests.Application
{
    /// <summary>
    /// Simple mock for <see cref="IContextFactory"/>. This class can be derived from by the test classes or by invoking constructor.
    /// <br/>It provides default implementation for all DbSets in <see cref="IApplicationDbContext"/> (empty).
    /// </summary>
    public class IContextFactoryMockFixtures
    {
        public Mock<IContextFactory> ContextFactoryMock { get; }
        public Mock<IApplicationDbContext> ContextMock { get; }

        /// <summary>
        /// Creates new instance of <see cref="IContextFactoryMockFixtures"/>. Can be supplied with custom <see cref="IApplicationDbContext"/> mock.<br/>
        /// By default, save changes (async) will return 1 if no custom <paramref name="premadeContextMock"/> was provided. This can be set by providing custom <paramref name="saveChangesOutcome"/>.
        /// </summary>
        /// <param name="premadeContextMock"></param>
        /// <param name="saveChangesOutcome"></param>
        public IContextFactoryMockFixtures(Mock<IApplicationDbContext> premadeContextMock = null, int saveChangesOutcome = -1)
        {
            ContextFactoryMock = new Mock<IContextFactory>();
            ContextMock = premadeContextMock ?? new Mock<IApplicationDbContext>();
            if (premadeContextMock is null)
            {
                ContextMock.Setup(x => x.SaveChanges())
                    .Returns(saveChangesOutcome != -1 ? saveChangesOutcome : 1);
                ContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(saveChangesOutcome != -1 ? saveChangesOutcome : 1);
            }
            ContextMock.DefaultValueProvider = new DefaultDbContextValueProvider();

            ContextFactoryMock.Setup(m => m.CreateDbContext()).Returns(ContextMock.Object).Callback(() => _disposeCount += 1);
            ContextMock.Setup(x => x.DisposeAsync()).Callback(() => _disposeCount -= 1);
            ContextMock.Setup(x => x.Dispose()).Callback(() => _disposeCount -= 1);
        }

        private int _disposeCount;
        public bool AllContextsDisposed => _disposeCount <= 0;
    }

    internal class DefaultDbContextValueProvider : DefaultValueProvider
    {
        protected override object GetDefaultValue(Type type, Mock mock)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DbSet<>))
            {
                var t = type.GetGenericArguments().First();
                dynamic dbSet = Activator.CreateInstance(typeof(List<>).MakeGenericType(t), args: 0);
                var queryable = Queryable.AsQueryable(dbSet);
                return MoqExtensions.BuildMockDbSet(queryable).Object;
            }

            return type.GetDefaultValue();
        }
    }
}