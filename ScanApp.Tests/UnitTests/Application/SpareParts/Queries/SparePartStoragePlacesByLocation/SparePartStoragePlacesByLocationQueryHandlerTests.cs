using FluentAssertions;
using MediatR;
using MockQueryable.Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.SpareParts.Queries.SparePartStoragePlacesByLocation;
using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.SpareParts.Queries.SparePartStoragePlacesByLocation
{
    public class SparePartStoragePlacesByLocationQueryHandlerTests : IContextFactoryMockFixtures
    {
        [Fact]
        public void Will_create_instance()
        {
            var subject = new SparePartStoragePlacesByLocationQueryHandler(ContextFactoryMock.Object);

            subject.Should().BeOfType<SparePartStoragePlacesByLocationQueryHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_if_no_context_factory_is_provided()
        {
            Action act = () => _ = new SparePartStoragePlacesByLocationQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        private static IEnumerable<SparePartStoragePlace> StoragePlaces => new List<SparePartStoragePlace>
        {
            new() { Id = "1", LocationId = "location_a", Name = "name_a" },
            new() { Id = "2", LocationId = "location_a", Name = "name_b" },
            new() { Id = "3", LocationId = "location_b", Name = "name_c" }
        };

        [Fact]
        public async Task Returns_all_spare_part_storage_places_for_given_location_as_valid_result_containing_RepairWorkshopModel_collection()
        {
            var dataMock = StoragePlaces.AsQueryable().BuildMockDbSet();
            ContextMock.SetupGet(c => c.SparePartStoragePlaces).Returns(dataMock.Object);

            var subject = new SparePartStoragePlacesByLocationQueryHandler(ContextFactoryMock.Object);
            var result = await subject.Handle(new SparePartStoragePlacesByLocationQuery("location_a"), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEquivalentTo(StoragePlaces.Take(2), opt => opt.ExcludingMissingMembers());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("unknown_location_id")]
        public async Task Returns_valid_result_containing_empty_collection_if_no_records_match_given_location_id(string locationId)
        {
            var dataMock = StoragePlaces.AsQueryable().BuildMockDbSet();
            ContextMock.SetupGet(c => c.SparePartStoragePlaces).Returns(dataMock.Object);

            var subject = new SparePartStoragePlacesByLocationQueryHandler(ContextFactoryMock.Object);
            var result = await subject.Handle(new SparePartStoragePlacesByLocationQuery(locationId), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEmpty();
        }

        [Fact]
        public async Task Throws_if_exception_was_thrown_when_receiving_data()
        {
            ContextFactoryMock.Setup(c => c.CreateDbContext()).Throws<ArgumentNullException>();

            var subject = new SparePartStoragePlacesByLocationQueryHandler(ContextFactoryMock.Object);
            Func<Task> act = async () => await subject.Handle(new SparePartStoragePlacesByLocationQuery("location_id"), CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Returns_invalid_result_of_cancelled_if_cancellation_occurred()
        {
            var token = new CancellationTokenSource(0).Token;
            ContextMock.SetupGet(c => c.SparePartStoragePlaces).Throws<OperationCanceledException>();

            var subject = new SparePartStoragePlacesByLocationQueryHandler(ContextFactoryMock.Object);
            var result = await subject.Handle(new SparePartStoragePlacesByLocationQuery("location_id"), token);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Cancelled);
            result.ErrorDescription.Exception.Should().BeOfType<OperationCanceledException>();
        }
    }
}