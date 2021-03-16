﻿using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ScanApp.Infrastructure.Identity
{
    public class LocationManagerService : ILocationManager
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

        public async Task<Result<Location>> GetLocationByName(string name)
        {
            await using var ctx = _ctxFactory.CreateDbContext();
            var location = await GetLocationBy(ctx, l => l.Name.Equals(name)).ConfigureAwait(false);

            return location is null
                ? new Result<Location>(ErrorType.NotFound, $"Location {name} does not exist in database")
                : new Result<Location>(location);
        }

        public async Task<Result<Location>> GetLocationById(string index)
        {
            await using var ctx = _ctxFactory.CreateDbContext();
            var location = await GetLocationBy(ctx, l => l.Id.Equals(index)).ConfigureAwait(false);

            return location is null
                ? new Result<Location>(ErrorType.NotFound, $"No location with index of {index}")
                : new Result<Location>(location);
        }

        private static Task<Location> GetLocationBy(IApplicationDbContext ctx, Expression<Func<Location, bool>> predicate) =>
            ctx.Locations.AsNoTracking().SingleOrDefaultAsync(predicate);

        public Task<Result<Location>> AddNewLocation(Location location)
        {
            _ = location ?? throw new ArgumentNullException(nameof(location));

            return string.IsNullOrEmpty(location.Id)
                ? AddNewLocation(location.Name)
                : Task.FromResult(new Result<Location>(ErrorType.NotValid, "Custom index value is not supported when adding location - set location index to null or empty string"));
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

            var users = await ctx.UserLocations
                .Where(o => o.LocationId.Equals(location.Id))
                .Join(ctx.Users, locations => locations.UserId, user => user.Id,
                    (_, user) => new ApplicationUser { Id = user.Id, ConcurrencyStamp = user.ConcurrencyStamp })
                .ToListAsync()
                .ConfigureAwait(false);

            if (await ctx.Locations.SingleOrDefaultAsync(l => l.Id.Equals(location.Id)).ConfigureAwait(false) is not null)
                ctx.Locations.Remove(location);

            ctx.AttachRange(users);
            foreach (var applicationUser in users)
            {
                applicationUser.ConcurrencyStamp = Guid.NewGuid().ToString();
            }

            await ctx.SaveChangesAsync().ConfigureAwait(false);
            return new Result<Location>(ResultType.Deleted);
        }
    }
}