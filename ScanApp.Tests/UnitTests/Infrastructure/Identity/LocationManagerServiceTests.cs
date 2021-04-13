using Microsoft.EntityFrameworkCore;
using Moq;
using ScanApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MockQueryable.Moq;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Infrastructure.Identity;
using Xunit;

namespace ScanApp.Tests.UnitTests.Infrastructure.Identity
{
    public class LocationManagerServiceTests
    {
        [Fact]
        public void Will_create_manager()
        {
            var contextFactoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();

            var sut = new LocationManagerService(contextFactoryMock.Object);

            sut.Should().NotBeNull()
                .And.BeOfType<LocationManagerService>()
                .And.BeAssignableTo<ILocationManager>();
        }

        [Fact]
        public void Throws_arg_null_if_no_context_factory_was_given_in_constructor()
        {
            Action act = () => new LocationManagerService(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GetAllLocations_will_return_all_locations()
        {
            var id = Guid.NewGuid().ToString();
            var contextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id);
            var locations = new List<Location>
            {
                new("location_a"),
                new("location_b")
            };

            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Locations.AddRange(locations);
                ctx.SaveChanges();
            }

            var sut = new LocationManagerService(contextFactoryMock.Object);

            var result = await sut.GetAllLocations();

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEquivalentTo(locations);
        }
    }
}
