using FluentAssertions;
using MediatR;
using MockQueryable.Moq;
using Moq;
using ScanApp.Application.Admin;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.SpareParts.Queries.SparePartStoragePlacesForCurrentUser;
using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScanApp.Application.Common.Helpers.Result;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.SpareParts.Queries.SparePartStoragePlacesForCurrentUser
{
    public class SparePartStoragePlacesForCurrentUserHandlerTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            var userServiceMock = new Mock<ICurrentUserService>();
            var subject = new SparePartStoragePlacesForCurrentUserHandler(userServiceMock.Object, contextFactoryMock.Object);

            subject.Should().BeOfType<SparePartStoragePlacesForCurrentUserHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Will_throw_arg_null_exc_if_not_given_CurrentUserService()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            Action act = () => _ = new SparePartStoragePlacesForCurrentUserHandler(null, contextFactoryMock.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Will_throw_arg_null_exc_if_not_given_ContextFactory()
        {
            var userServiceMock = new Mock<ICurrentUserService>();
            Action act = () => _ = new SparePartStoragePlacesForCurrentUserHandler(userServiceMock.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }

        private static IEnumerable<SparePartStoragePlace> StoragePlaces => new List<SparePartStoragePlace>
        {
            new() { Id = "1", LocationId = "location_id_a", Name = "name_a" },
            new() { Id = "2", LocationId = "location_id_b", Name = "name_b" },
            new() { Id = "3", LocationId = "location_id_b", Name = "name_c" }
        };

        [Theory]
        [InlineData("location_id_a")]
        [InlineData("location_id_b")]
        public async Task Returns_valid_result_with_RepairWorkshopModel_collection_based_on_current_user_location_claim(string userLocation)
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            var contextMock = new Mock<IApplicationDbContext>();
            var dataMock = StoragePlaces.AsQueryable().BuildMockDbSet();
            contextFactoryMock.Setup(f => f.CreateDbContext()).Returns(contextMock.Object);
            contextMock.SetupGet(c => c.SparePartStoragePlaces).Returns(dataMock.Object);
            var userServiceMock = new Mock<ICurrentUserService>();
            userServiceMock
                .Setup(u => u.HasClaim(Globals.ClaimTypes.IgnoreLocation, Globals.ModuleNames.SparePartsModule))
                .ReturnsAsync(false);
            userServiceMock.Setup(u => u.AllClaims(Globals.ClaimTypes.Location))
                .ReturnsAsync(new List<ClaimModel> { new(Globals.ClaimTypes.Location, userLocation) });

            var subject = new SparePartStoragePlacesForCurrentUserHandler(userServiceMock.Object, contextFactoryMock.Object);
            var result = await subject.Handle(new SparePartStoragePlacesForCurrentUserQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEquivalentTo(StoragePlaces.Where(s => s.LocationId == userLocation),
                opt => opt.ExcludingMissingMembers());
        }

        [Fact]
        public async Task Returns_valid_result_with_empty_collection_if_no_storage_place_matches_user_location_claim()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            var contextMock = new Mock<IApplicationDbContext>();
            var dataMock = StoragePlaces.AsQueryable().BuildMockDbSet();
            contextFactoryMock.Setup(f => f.CreateDbContext()).Returns(contextMock.Object);
            contextMock.SetupGet(c => c.SparePartStoragePlaces).Returns(dataMock.Object);
            var userServiceMock = new Mock<ICurrentUserService>();
            userServiceMock
                .Setup(u => u.HasClaim(Globals.ClaimTypes.IgnoreLocation, Globals.ModuleNames.SparePartsModule))
                .ReturnsAsync(false);
            userServiceMock.Setup(u => u.AllClaims(Globals.ClaimTypes.Location))
                .ReturnsAsync(new List<ClaimModel> { new(Globals.ClaimTypes.Location, "unknown_location_id") });

            var subject = new SparePartStoragePlacesForCurrentUserHandler(userServiceMock.Object, contextFactoryMock.Object);
            var result = await subject.Handle(new SparePartStoragePlacesForCurrentUserQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEmpty();
        }

        [Fact]
        public async Task Returns_valid_result_with_empty_collection_if_user_has_no_location_claim()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            var contextMock = new Mock<IApplicationDbContext>();
            var dataMock = StoragePlaces.AsQueryable().BuildMockDbSet();
            contextFactoryMock.Setup(f => f.CreateDbContext()).Returns(contextMock.Object);
            contextMock.SetupGet(c => c.SparePartStoragePlaces).Returns(dataMock.Object);
            var userServiceMock = new Mock<ICurrentUserService>();
            userServiceMock
                .Setup(u => u.HasClaim(Globals.ClaimTypes.IgnoreLocation, Globals.ModuleNames.SparePartsModule))
                .ReturnsAsync(false);
            userServiceMock.Setup(u => u.AllClaims(Globals.ClaimTypes.Location))
                .ReturnsAsync(new List<ClaimModel>(0));

            var subject = new SparePartStoragePlacesForCurrentUserHandler(userServiceMock.Object, contextFactoryMock.Object);
            var result = await subject.Handle(new SparePartStoragePlacesForCurrentUserQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEmpty();
        }

        [Fact]
        public async Task Returns_valid_result_with_all_storage_places_as_RepairWorkshopModels_if_user_has_claim_ignore_location_for_spare_parts()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            var contextMock = new Mock<IApplicationDbContext>();
            var dataMock = StoragePlaces.AsQueryable().BuildMockDbSet();
            contextFactoryMock.Setup(f => f.CreateDbContext()).Returns(contextMock.Object);
            contextMock.SetupGet(c => c.SparePartStoragePlaces).Returns(dataMock.Object);
            var userServiceMock = new Mock<ICurrentUserService>();
            userServiceMock
                .Setup(u => u.HasClaim(Globals.ClaimTypes.IgnoreLocation, Globals.ModuleNames.SparePartsModule))
                .ReturnsAsync(true);

            var subject = new SparePartStoragePlacesForCurrentUserHandler(userServiceMock.Object, contextFactoryMock.Object);
            var result = await subject.Handle(new SparePartStoragePlacesForCurrentUserQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEquivalentTo(StoragePlaces, opt => opt.ExcludingMissingMembers());
        }

        [Fact]
        public async Task Throws_if_exception_is_thrown_during_execution()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            var userServiceMock = new Mock<ICurrentUserService>();
            contextFactoryMock.Setup(f => f.CreateDbContext()).Throws<Exception>();

            var subject = new SparePartStoragePlacesForCurrentUserHandler(userServiceMock.Object, contextFactoryMock.Object);

            Func<Task> act = async () => await subject.Handle(new SparePartStoragePlacesForCurrentUserQuery(), CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task Returns_invalid_result_of_cancelled_with_empty_collection_if_cancelled()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            var contextMock = new Mock<IApplicationDbContext>();
            contextFactoryMock.Setup(f => f.CreateDbContext()).Returns(contextMock.Object);
            contextMock.SetupGet(c => c.SparePartStoragePlaces).Throws<OperationCanceledException>();
            var userServiceMock = new Mock<ICurrentUserService>();
            userServiceMock
                .Setup(u => u.HasClaim(Globals.ClaimTypes.IgnoreLocation, Globals.ModuleNames.SparePartsModule))
                .ReturnsAsync(true);
            var token = new CancellationTokenSource(0).Token;

            var subject = new SparePartStoragePlacesForCurrentUserHandler(userServiceMock.Object, contextFactoryMock.Object);
            var result = await subject.Handle(new SparePartStoragePlacesForCurrentUserQuery(), token);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Cancelled);
            result.ErrorDescription.Exception.Should().BeOfType<OperationCanceledException>();
        }
    }
}