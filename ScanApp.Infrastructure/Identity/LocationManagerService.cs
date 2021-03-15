using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Domain.Entities;
using ScanApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Infrastructure.Identity
{
    public class LocationManagerService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;

        public LocationManagerService(IDbContextFactory<ApplicationDbContext> ctxFactory)
        {
            _ctxFactory = ctxFactory ?? throw new ArgumentNullException(nameof(ctxFactory));
        }

        public async Task<Result<List<Location>>> GetAllLocations()
        {
            await using var ctx = _ctxFactory.CreateDbContext();
            var locations = await ctx.Locations.AsNoTracking().ToListAsync().ConfigureAwait(false);
            return new Result<List<Location>>(locations);
        }

        public async Task<Result<Location>> GetLocationsByIndex(int index)
        {
            await using var ctx = _ctxFactory.CreateDbContext();
            var location = await ctx.Locations
                .AsNoTracking()
                .Where(l => l.Id == index)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            return location is null
                ? new Result<Location>(ErrorType.NotFound, $"No location with index of {index}")
                : new Result<Location>(location);
        }

        public Task<Result<Location>> AddNewLocation(Location location)
        {
            _ = location ?? throw new ArgumentNullException(nameof(location));

            return location.Id != 0
                ? Task.FromResult(new Result<Location>(ErrorType.NotValid, "Custom index value is not supported when adding location - set location index to 0"))
                : AddNewLocation(location.Name);
        }

        public async Task<Result<Location>> AddNewLocation(string locationName)
        {
            await using var ctx = _ctxFactory.CreateDbContext();

            var newLocation = new Location(locationName);
            var existingName = await ctx.Locations
                .AsNoTracking()
                .Select(l => l.NormalizedName)
                .Where(n => n.Equals(newLocation.NormalizedName))
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (existingName is not null)
                return new Result<Location>(ErrorType.Duplicated, $"Location {locationName} already exist in the database");

            await ctx.Locations.AddAsync(newLocation).ConfigureAwait(false);
            var change = await ctx.SaveChangesAsync().ConfigureAwait(false);
            return change != 0
                ? new Result<Location>(ResultType.Created, newLocation)
                : new Result<Location>(ErrorType.Unknown, $"Could not save changes in context for new location {locationName}");
        }

        public async Task<Result> RemoveLocation(Location location)
        {
            await using var ctx = _ctxFactory.CreateDbContext();

            ctx.Locations.Remove(location);

            var users = await ctx.Users
                .AsNoTracking()
                .Where(u => u.LocationId == location.Id)
                .Select(u => u.Id)
                .ToListAsync();

            var userEntities = users.ConvertAll(id => new ApplicationUser() { Id = id, LocationId = 0 });
            ctx.Users.AttachRange(userEntities);
            userEntities.ForEach(user => ctx.Entry(user).Property(u => u.LocationId).IsModified = true);

            var change = await ctx.SaveChangesAsync();
            return change != 0
                ? new Result<Location>(ResultType.Deleted)
                : new Result<Location>(ErrorType.Unknown, $"Could not delete {location.Name}");
        }
    }
}