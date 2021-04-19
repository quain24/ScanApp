using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.SpareParts.Commands.CreateSpareParts;
using ScanApp.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Application.SpareParts.Commands.CreateSpareParts
{
    public class CreateSparePartsCommandHandlerTests : SqlLiteInMemoryDbFixture
    {
        public ITestOutputHelper Output { get; }

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

            var sut = new CreateSparePartsCommandHandler(ctxFactoryMock.Object);

            var result = await sut.Handle(request, CancellationToken.None);

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

            var sut = new CreateSparePartsCommandHandler(ctxFactoryMock.Object);

            var result = await sut.Handle(request, CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.DatabaseError);
            using var context = NewDbContext;
            result.ErrorDescription.Exception.Should().BeOfType<DbUpdateException>();
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

            var sut = new CreateSparePartsCommandHandler(ctxFactoryMock.Object);

            var result = await sut.Handle(request, CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.DatabaseError);
            result.ErrorDescription.Exception.Should().BeOfType<DbUpdateException>();
            using var context = NewDbContext;
            context.SpareParts.Should().BeEmpty();
        }
    }
}