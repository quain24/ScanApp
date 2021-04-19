using FluentAssertions;
using MediatR;
using MockQueryable.Moq;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
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
    public class GetAllSparePartStoragePlacesQueryHandlerTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            var subject = new GetAllSparePartStoragePlacesQueryHandler(contextFactoryMock.Object);

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
            var contextFactoryMock = new Mock<IContextFactory>();
            var contextMock = new Mock<IApplicationDbContext>();
            contextFactoryMock.Setup(c => c.CreateDbContext()).Returns(contextMock.Object);
            var data = new List<SparePartStoragePlace>
            {
                new(){Id = "1", LocationId = "location_a", Name = "name_a"},
                new(){Id = "2", LocationId = "location_b", Name = "name_b"},
                new(){Id = "3", LocationId = "location_c", Name = "name_c"}
            };
            var dataMock = data.AsQueryable().BuildMockDbSet();
            contextMock.SetupGet(c => c.SparePartStoragePlaces).Returns(dataMock.Object);

            var subject = new GetAllSparePartStoragePlacesQueryHandler(contextFactoryMock.Object);
            var result = await subject.Handle(new GetAllSparePartStoragePlacesQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEquivalentTo(data, opt => opt.ExcludingMissingMembers());
        }

        [Fact]
        public async Task Throws_if_exception_was_thrown_when_receiving_data()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            contextFactoryMock.Setup(c => c.CreateDbContext()).Throws<ArgumentNullException>();

            var subject = new GetAllSparePartStoragePlacesQueryHandler(contextFactoryMock.Object);
            Func<Task> act = async () => await subject.Handle(new GetAllSparePartStoragePlacesQuery(), CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Returns_invalid_result_of_cancelled_if_cancellation_occurred()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            var contextMock = new Mock<IApplicationDbContext>();
            contextFactoryMock.Setup(c => c.CreateDbContext()).Returns(contextMock.Object);
            var token = new CancellationTokenSource(0).Token;
            contextMock.SetupGet(c => c.SparePartStoragePlaces).Throws<OperationCanceledException>();

            var subject = new GetAllSparePartStoragePlacesQueryHandler(contextFactoryMock.Object);
            var result = await subject.Handle(new GetAllSparePartStoragePlacesQuery(), token);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Cancelled);
            result.ErrorDescription.Exception.Should().BeOfType<OperationCanceledException>();
        }
    }
}