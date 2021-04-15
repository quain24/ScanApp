using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Infrastructure.Identity;
using ScanApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [Fact]
        public async Task GetLocationByName_will_return_valid_result_of_location_with_given_name()
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

            var result = await sut.GetLocationByName(locations[0].Name);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().NotBeNull()
                .And.BeEquivalentTo(new Location(locations[0].Id, locations[0].Name));
        }

        [Fact]
        public async Task GetLocationByName_will_return_invalid_result_of_not_found_if_no_location_with_given_name_exists()
        {
            var id = Guid.NewGuid().ToString();
            var contextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id);

            var sut = new LocationManagerService(contextFactoryMock.Object);

            var result = await sut.GetLocationByName("name");

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            result.Output.Should().BeNull();
        }

        [Fact]
        public async Task GetLocationById_will_return_valid_result_of_location_with_given_id()
        {
            var id = Guid.NewGuid().ToString();
            var contextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id);
            var locations = new List<Location>
            {
                new("1","location_a"),
                new("2","location_b")
            };

            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Locations.AddRange(locations);
                ctx.SaveChanges();
            }

            var sut = new LocationManagerService(contextFactoryMock.Object);

            var result = await sut.GetLocationById(locations[0].Id);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().NotBeNull()
                .And.BeEquivalentTo(new Location(locations[0].Id, locations[0].Name));
        }

        [Fact]
        public async Task GetLocationById_will_return_invalid_result_of_not_found_if_no_location_with_given_id_exists()
        {
            var id = Guid.NewGuid().ToString();
            var contextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id);

            var sut = new LocationManagerService(contextFactoryMock.Object);

            var result = await sut.GetLocationById("id");

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
            result.Output.Should().BeNull();
        }

        [Theory]
        [InlineData("new_id", "new_name")]
        [InlineData(null, "new_name")]
        public async Task AddNewLocation_will_add_new_location(string locationId, string name)
        {
            var id = Guid.NewGuid().ToString();
            var contextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id);
            var locations = new List<Location>
            {
                new("1","location_a"),
                new("2","location_b")
            };

            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Locations.AddRange(locations);
                ctx.SaveChanges();
            }

            var sut = new LocationManagerService(contextFactoryMock.Object);

            var result = await sut.AddNewLocation(new Location(locationId, name));

            result.Conclusion.Should().BeTrue();
            result.ResultType.Should().Be(ResultType.Created);
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Locations.Should().HaveCount(3);
                ctx.Locations.Should().ContainEquivalentOf(new Location(locationId, name), opt =>
                    {
                        opt.ExcludingMissingMembers();
                        if (locationId is null)
                            opt.Excluding(e => e.Id);
                        return opt;
                    });
            }
        }

        [Fact]
        public async Task AddNewLocation_will_add_new_location_given_only_name()
        {
            var id = Guid.NewGuid().ToString();
            var contextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id);
            var locations = new List<Location>
            {
                new("1","location_a"),
                new("2","location_b")
            };

            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Locations.AddRange(locations);
                ctx.SaveChanges();
            }

            var sut = new LocationManagerService(contextFactoryMock.Object);

            var result = await sut.AddNewLocation("new_name");

            result.Conclusion.Should().BeTrue();
            result.ResultType.Should().Be(ResultType.Created);
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Locations.Should().HaveCount(3);
                ctx.Locations.Should().ContainEquivalentOf(new Location("new_name"), opt =>
                {
                    opt.ExcludingMissingMembers();
                    opt.Excluding(e => e.Id);
                    return opt;
                });
            }
        }

        [Fact]
        public async Task AddNewLocation_will_return_invalid_result_of_duplicated_if_tried_to_add_location_with_name_and_id_that_exists()
        {
            var id = Guid.NewGuid().ToString();
            var contextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id);
            var locations = new List<Location>
            {
                new("1","location_a"),
                new("2","location_b")
            };

            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Locations.AddRange(locations);
                ctx.SaveChanges();
            }

            var sut = new LocationManagerService(contextFactoryMock.Object);

            var result = await sut.AddNewLocation(locations[0]);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Duplicated);
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Locations.Should().HaveCount(2);
                ctx.Locations.Should().BeEquivalentTo(locations, opt => opt.ExcludingMissingMembers());
            }
        }

        [Fact]
        public async Task AddNewLocation_will_return_invalid_result_of_duplicated_if_tried_to_add_location_with_name_that_exists()
        {
            var id = Guid.NewGuid().ToString();
            var contextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id);
            var locations = new List<Location>
            {
                new("1","location_a"),
                new("2","location_b")
            };

            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Locations.AddRange(locations);
                ctx.SaveChanges();
            }

            var sut = new LocationManagerService(contextFactoryMock.Object);

            var result = await sut.AddNewLocation(locations[0].Name);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Duplicated);
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Locations.Should().HaveCount(2);
                ctx.Locations.Should().BeEquivalentTo(locations, opt => opt.ExcludingMissingMembers());
            }
        }

        [Fact]
        public async Task RemoveLocation_will_remove_location_and_change_affected_users_concurrency_stamp()
        {
            var id = Guid.NewGuid().ToString();
            var contextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id);
            var locations = new List<Location>
            {
                new("1","location_a"),
                new("2","location_b")
            };
            var user = UserGeneratorFixture.CreateValidUser();
            var userLocation = new UserLocation { LocationId = locations[0].Id, UserId = user.Id };

            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Users.Add(user);
                ctx.Locations.AddRange(locations);
                ctx.UserLocations.Add(userLocation);
                ctx.SaveChanges();
            }

            var userOriginalConcurrencyStamp = user.ConcurrencyStamp;

            var sut = new LocationManagerService(contextFactoryMock.Object);

            var result = await sut.RemoveLocation(locations[0]);

            result.Conclusion.Should().BeTrue();
            result.ResultType.Should().Be(ResultType.Deleted);
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Locations.AsNoTracking().Should().HaveCount(1)
                    .And.NotContain(l => l.Id == locations[0].Id);
                ctx.Users.AsNoTracking().First().ConcurrencyStamp.Should().NotBe(userOriginalConcurrencyStamp);
                // Not checking if UserLocation deleted - EF inMemory does not support relationships - SQL database will delete it
            }
        }

        [Fact]
        public async Task RemoveLocation_will_return_valid_result_of_delete_even_if_there_is_no_location_matched_to_delete()
        {
            var id = Guid.NewGuid().ToString();
            var contextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id);
            var locations = new List<Location>
            {
                new("1","location_a"),
                new("2","location_b")
            };
            var user = UserGeneratorFixture.CreateValidUser();
            var userLocation = new UserLocation { LocationId = locations[0].Id, UserId = user.Id };

            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Users.Add(user);
                ctx.Locations.AddRange(locations);
                ctx.UserLocations.Add(userLocation);
                ctx.SaveChanges();
            }

            var sut = new LocationManagerService(contextFactoryMock.Object);

            var result = await sut.RemoveLocation(new Location("3", "location_c"));

            result.Conclusion.Should().BeTrue();
            result.ResultType.Should().Be(ResultType.Deleted);
            using (var ctx = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock(id).Object.CreateDbContext())
            {
                ctx.Locations.AsNoTracking().Should().BeEquivalentTo(locations);
            }
        }

        [Fact]
        public async Task RemoveLocation_will_return_invalid_result_of_not_valid_if_given_location_does_not_have_id()
        {
            var contextFactoryMock = AppDbContextFactoryMockFixture.CreateSimpleFactoryMock();
            var sut = new LocationManagerService(contextFactoryMock.Object);

            var result = await sut.RemoveLocation(new Location("location_c"));

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotValid);
        }
    }
}