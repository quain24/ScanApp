using EntityFramework.Exceptions.Common;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.SpareParts.Commands.CreateSpareParts;
using ScanApp.Domain.Entities;
using ScanApp.Tests.UnitTests.Application;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.IntegrationTests.Application.SpareParts.Commands.CreateSpareParts
{
    public class CreateSparePartsCommandHandlerTests : DbFixtureWithExceptionHandlers
    {
        public CreateSparePartsCommandHandlerTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public async Task DatabaseIsAvailableAndCanBeConnectedTo()
        {
            Assert.True(await NewDbContext.Database.CanConnectAsync());
        }

        [Fact]
        public async Task Will_add_new_spare_part_to_database()
        {
            var sparePartType = new SparePartType("part_type");
            var location = new Location("location_name");
            var sparePartStoragePlace = new SparePartStoragePlace { Name = "storage_place_name" };
            using (var ctx = NewDbContext)
            {
                ctx.SparePartTypes.Add(sparePartType);
                ctx.Locations.Add(location);
                // saving to auto generate id's to be used below
                ctx.SaveChanges();
                sparePartStoragePlace.LocationId = location.Id;
                ctx.SparePartStoragePlaces.Add(sparePartStoragePlace);
                ctx.SaveChanges();
            }
            var ctxFactoryMock = new Mock<IContextFactory>();
            ctxFactoryMock.Setup(c => c.CreateDbContext()).Returns(NewDbContext);

            var sparePart = new SparePartModel(sparePartType.Name, 1, "article_id", sparePartStoragePlace.Id);
            var request = new CreateSparePartsCommand(sparePart);

            var result = await Provider.GetService<IMediator>().Send(request);

            result.Conclusion.Should().BeTrue();
            result.ResultType.Should().Be(ResultType.Created);
            using var context = NewDbContext;
            context.SpareParts.Should().HaveCount(1)
                .And.Subject.First().Should().BeEquivalentTo(sparePart, opt => opt.ExcludingMissingMembers());
        }

        [Fact]
        public async Task Wont_add_new_spare_part_given_storage_place_id_does_not_exist_in_db()
        {
            var sparePartType = new SparePartType("part_type");
            var location = new Location("location_name");
            var sparePartStoragePlace = new SparePartStoragePlace { Name = "storage_place_name" };
            using (var ctx = NewDbContext)
            {
                ctx.SparePartTypes.Add(sparePartType);
                ctx.Locations.Add(location);
                // saving to auto generate id's to be used below
                ctx.SaveChanges();
                sparePartStoragePlace.LocationId = location.Id;
                ctx.SparePartStoragePlaces.Add(sparePartStoragePlace);
                ctx.SaveChanges();
            }
            var ctxFactoryMock = new Mock<IContextFactory>();
            ctxFactoryMock.Setup(c => c.CreateDbContext()).Returns(NewDbContext);

            var sparePart = new SparePartModel(sparePartType.Name, 1, "article_id", "unknown_id");
            var request = new CreateSparePartsCommand(sparePart);
            var result = await Provider.GetService<IMediator>().Send(request);

            var sut = new CreateSparePartsCommandHandler(ctxFactoryMock.Object);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ReferenceConstraint);
            using var context = NewDbContext;
            result.ErrorDescription.Exception.Should().BeOfType<ReferenceConstraintException>();
            context.SpareParts.Should().BeEmpty();
        }

        [Fact]
        public async Task Wont_add_new_spare_part_given_name_does_not_exist_in_database()
        {
            var sparePartType = new SparePartType("part_type");
            var location = new Location("location_name");
            var sparePartStoragePlace = new SparePartStoragePlace { Name = "storage_place_name" };
            using (var ctx = NewDbContext)
            {
                ctx.SparePartTypes.Add(sparePartType);
                ctx.Locations.Add(location);
                // saving to auto generate id's to be used below
                ctx.SaveChanges();
                sparePartStoragePlace.LocationId = location.Id;
                ctx.SparePartStoragePlaces.Add(sparePartStoragePlace);
                ctx.SaveChanges();
            }
            var ctxFactoryMock = new Mock<IContextFactory>();
            ctxFactoryMock.Setup(c => c.CreateDbContext()).Returns(NewDbContext);

            var sparePart = new SparePartModel("unknown_part_type_name", 1, "article_id", sparePartStoragePlace.Id);
            var request = new CreateSparePartsCommand(sparePart);

            var result = await Provider.GetService<IMediator>().Send(request);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.ReferenceConstraint);
            result.ErrorDescription.Exception.Should().BeOfType<ReferenceConstraintException>();
            using var context = NewDbContext;
            context.SpareParts.Should().BeEmpty();
        }

        [Theory]
        [InlineData(typeof(OperationCanceledException))]
        [InlineData(typeof(TaskCanceledException))]
        public async Task Returns_invalid_result_of_cancelled_on_cancellation_or_timeout(Type type)
        {
            dynamic exc = Activator.CreateInstance(type);
            var contextFactoryMock = new IContextFactoryMockFixtures().ContextFactoryMock;
            contextFactoryMock.Setup(m => m.CreateDbContext()).Throws(exc);

            ServiceCollection.Replace(ServiceDescriptor.Singleton<IContextFactory>(contextFactoryMock.Object));
            var result = await Provider.GetService<IMediator>().Send(new CreateSparePartsCommand());

            using var scope = new AssertionScope();
            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Canceled);
            result.ErrorDescription.Exception.Should().BeOfType(type);
        }
    }
}