using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Infrastructure.Persistence;
using ScanApp.Infrastructure.Services;
using System;
using Xunit;

namespace ScanApp.Tests.UnitTests.Infrastructure.Services
{
    public class AppDbContextFactoryTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();

            var subject = new AppDbContextFactory(factoryMock.Object);

            subject.Should().NotBeNull()
                .And.BeOfType<AppDbContextFactory>()
                .And.BeAssignableTo<IContextFactory>();
        }

        [Fact]
        public void Will_throw_arg_null_if_no_dbcontext_factory_instance_is_provided()
        {
            Action act = () => new AppDbContextFactory(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Returns_dbcontext_instance()
        {
            var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            var subject = new AppDbContextFactory(factoryMock.Object);

            subject.CreateDbContext();

            factoryMock.Verify(f => f.CreateDbContext(), Times.Once);
        }
    }
}