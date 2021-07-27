using FluentAssertions;
using MediatR;
using MockQueryable.Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.SpareParts.Queries.GetAllSparePartStoragePlaces;
using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.SpareParts.Queries.GetAllSparePartStoragePlaces
{
    public class GetAllSparePartStoragePlacesQueryHandlerTests : IContextFactoryMockFixtures
    {
        [Fact]
        public void Will_create_instance()
        {
            var subject = new GetAllSparePartStoragePlacesQueryHandler(ContextFactoryMock.Object);

            subject.Should().BeOfType<GetAllSparePartStoragePlacesQueryHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_if_no_context_factory_is_provided()
        {
            Action act = () => _ = new GetAllSparePartStoragePlacesQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Returns_all_storage_places_as_valid_result_containing_RepairWorkshopModel_collection()
        {
            var data = new List<SparePartStoragePlace>
            {
                new(){Id = "1", LocationId = "location_a", Name = "name_a"},
                new(){Id = "2", LocationId = "location_b", Name = "name_b"},
                new(){Id = "3", LocationId = "location_c", Name = "name_c"}
            };
            var dataMock = data.AsQueryable().BuildMockDbSet();
            ContextMock.SetupGet(c => c.SparePartStoragePlaces).Returns(dataMock.Object);

            var subject = new GetAllSparePartStoragePlacesQueryHandler(ContextFactoryMock.Object);
            var result = await subject.Handle(new GetAllSparePartStoragePlacesQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEquivalentTo(data, opt => opt.ExcludingMissingMembers());
        }

        [Fact]
        public async Task Throws_if_exception_was_thrown_when_receiving_data()
        {
            ContextFactoryMock.Setup(c => c.CreateDbContext()).Throws<ArgumentNullException>();

            var subject = new GetAllSparePartStoragePlacesQueryHandler(ContextFactoryMock.Object);
            Func<Task> act = async () => await subject.Handle(new GetAllSparePartStoragePlacesQuery(), CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(OperationCanceledException))]
        [InlineData(typeof(TaskCanceledException))]
        public async Task Returns_invalid_result_of_cancelled_on_cancellation_or_timeout(Type type)
        {
            dynamic exc = Activator.CreateInstance(type);
            ContextFactoryMock.Setup(m => m.CreateDbContext()).Throws(exc);

            var subject = new GetAllSparePartStoragePlacesQueryHandler(ContextFactoryMock.Object);
            var result = await subject.Handle(new GetAllSparePartStoragePlacesQuery(), CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Canceled);
            result.ErrorDescription.Exception.Should().BeOfType(type);
        }
    }
}