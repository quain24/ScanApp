using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Tests.UnitTests.Application
{
    public class IContextFactoryMockFixtures
    {
        public Mock<IContextFactory> ContextFactoryMock { get; }
        public Mock<IApplicationDbContext> ContextMock { get; }

        public IContextFactoryMockFixtures(Mock<IApplicationDbContext> premadeContextMock = null)
        {
            ContextFactoryMock = new Mock<IContextFactory>();
            ContextMock = premadeContextMock ?? new Mock<IApplicationDbContext>();
            ContextMock.DefaultValueProvider = new DefaultDbContextValueProvider();

            ContextFactoryMock.Setup(m => m.CreateDbContext()).Returns(ContextMock.Object);
        }
    }

    public class DefaultDbContextValueProvider : DefaultValueProvider
    {
        protected override object GetDefaultValue(Type type, Mock mock)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DbSet<>))
            {
                var t = type.GetGenericArguments().First();
                dynamic dbSet = Activator.CreateInstance(typeof(List<>).MakeGenericType(t));
                var queryable = Queryable.AsQueryable(dbSet);
                return MoqExtensions.BuildMockDbSet(queryable).Object;
            }

            return type.GetDefaultValue();
        }
    }
}