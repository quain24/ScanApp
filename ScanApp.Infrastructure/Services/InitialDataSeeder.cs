using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScanApp.Application.Admin;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Infrastructure.Common.Exceptions;
using ScanApp.Infrastructure.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Infrastructure.Services
{
    public class InitialDataSeeder : IInitialDataSeeder
    {
        private const string ServiceName = "Seeder";
        private const string Administrator = Globals.AccountNames.Administrator;
        private const string AdministratorPwd = "initial";

        private readonly Action<string, string> _logInformation;

        private readonly Location[] _defaultLocations =
        {
            // ID's are encoded for policies that are using locations - they operate on id, not name
            new("DF5BBE28-688E-4DA8-8E79-3C1D9C75CFAA", "Sady"),
            new("80818ca4-57f2-4871-81d4-dc97aef5aa64", "Genderkingen")
        };

        private readonly SparePartStoragePlace[] _defaultSparePartStoragePlaces =
        {
            new() {LocationId = "DF5BBE28-688E-4DA8-8E79-3C1D9C75CFAA", Name = "100000"},
            new() {LocationId = "DF5BBE28-688E-4DA8-8E79-3C1D9C75CFAA", Name = "86001"},
            new() {LocationId = "80818ca4-57f2-4871-81d4-dc97aef5aa64", Name = "33000"},
        };

        private readonly string[] _defaultRoles =
        {
            Globals.RoleNames.Administrator,
            "User"
        };

        private readonly Claim[] _defaultClaims =
        {
            new(Globals.ClaimTypes.CanEdit, "Roles"),
            new(Globals.ClaimTypes.CanEdit, "Users"),
            new(Globals.ClaimTypes.CanEdit, Globals.ModuleNames.SparePartsModule),
            new(Globals.ClaimTypes.IgnoreLocation, Globals.ModuleNames.SparePartsModule)
        };

        private readonly Season _defaultSeason = new Season("Default", DateTime.MinValue, DateTime.MaxValue);

        private IUserManager UserManager { get; }

        private IUserInfo UserInfo { get; }
        private IRoleManager RoleManager { get; }
        private ILocationManager LocationManager { get; }
        private IDbContextFactory<ApplicationDbContext> ContextFactory { get; }
        private ILogger<InitialDataSeeder> Logger { get; }

        public InitialDataSeeder(IUserManager userManager,
            IUserInfo userInfo,
            IRoleManager roleManager,
            ILocationManager locationManager,
            IDbContextFactory<ApplicationDbContext> contextFactory,
            ILogger<InitialDataSeeder> logger)
        {
            UserManager = userManager;
            UserInfo = userInfo;
            RoleManager = roleManager;
            LocationManager = locationManager;
            ContextFactory = contextFactory;
            Logger = logger;

            _logInformation = (partName, info) => Logger.LogInformation("{name} - {part} - " + info, ServiceName, partName);
        }

        public async Task Initialize(bool force)
        {
            if (force)
                Logger.LogInformation("{name} - Reapplying initial data seed - FORCE parameter checked", ServiceName);
            await ApplyMigrations(force).ConfigureAwait(false);

            if (await UserInfo.UserExists(Administrator).ConfigureAwait(false) && !force)
                return;

            await AddDefaultLocations().ConfigureAwait(false);
            await AddDefaultSparePartStoragePlaces().ConfigureAwait(false);
            await AddDefaultUsers().ConfigureAwait(false);
            await AddDefaultRoles().ConfigureAwait(false);
            await AddDefaultClaimSourceList().ConfigureAwait(false);
            await AssignClaimsToAdministratorRole().ConfigureAwait(false);
            await AssignRolesToAdministrator().ConfigureAwait(false);
            await AddDefaultSeason().ConfigureAwait(false);
        }

        private async Task ApplyMigrations(bool force)
        {
            const string name = "Migrations";
            await using var ctx = ContextFactory.CreateDbContext();
            if ((await ctx.Database.GetPendingMigrationsAsync().ConfigureAwait(false)).Any() || force)
            {
                _logInformation(name, "Beginning applying pending database migrations");
                await ctx.Database.MigrateAsync().ConfigureAwait(false);
                _logInformation(name, "Migrations applied");
            }
        }

        private async Task AddDefaultLocations()
        {
            const string name = "Locations";
            _logInformation(name, "Beginning initial locations seeding");
            foreach (var location in _defaultLocations)
            {
                var res = await LocationManager.AddNewLocation(location).ConfigureAwait(false);
                HandleResult(res, name);
            }
            _logInformation(name, "Initial locations seeded");
        }

        private async Task AddDefaultSparePartStoragePlaces()
        {
            const string name = "Spare part locations";
            _logInformation(name, "Beginning initial seeding");
            await using var ctx = ContextFactory.CreateDbContext();
            try
            {
                foreach (var spsp in _defaultSparePartStoragePlaces)
                {
                    if (ctx.SparePartStoragePlaces.AsNoTracking().Any(s => s.Name.Equals(spsp.LocationId)))
                        continue;
                    ctx.Add(spsp);
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
                _logInformation(name, "seeding completed");
            }
            catch (DbUpdateException)
            {
                Logger.LogError("{name} - {part} - seeding failed!", ServiceName, name);
                throw;
            }
        }

        private async Task AddDefaultUsers()
        {
            _logInformation("Default admin", "Seeding Default administrator account");
            var res = await UserManager.AddNewUser
            (
                userName: Administrator,
                password: AdministratorPwd,
                email: "Administrator@raumshmiede.de",
                phoneNumber: "+111111111111",
                location: _defaultLocations[0],
                canBeLockedOut: true
            ).ConfigureAwait(false);

            HandleResult(res, "Users");
            _logInformation("Default admin", "Default admin account created");
        }

        private async Task AddDefaultRoles()
        {
            const string name = "Roles";
            _logInformation(name, "Seeding Default roles");
            foreach (var role in _defaultRoles)
            {
                var res = await RoleManager.AddNewRole(role).ConfigureAwait(false);
                HandleResult(res, name);
            }
            _logInformation(name, "Default roles created");
        }

        private async Task AddDefaultClaimSourceList()
        {
            const string name = "Default claims";
            try
            {
                _logInformation(name, "Inserting default claim collection to database");
                await using var ctx = ContextFactory.CreateDbContext();
                foreach (var claim in _defaultClaims)
                {
                    if (await ctx.ClaimsSource.AsNoTracking().AnyAsync(c => c.Value == claim.Value && c.Type == claim.Type).ConfigureAwait(false))
                        continue;
                    ctx.Add(claim);
                }
                await ctx.SaveChangesAsync().ConfigureAwait(false);
                _logInformation(name, "Inserted");
            }
            catch (DbUpdateException ex)
            {
                Logger.LogError(ex, "{name} - {part} - Exception thrown during initial seeding", ServiceName, name);
                throw;
            }
        }

        private async Task AssignClaimsToAdministratorRole()
        {
            const string name = "Administrator role claims";
            _logInformation(name, "Assigning all claims to administrator role");
            foreach (var claim in _defaultClaims)
            {
                var claimModel = new ClaimModel(claim.Type, claim.Value);
                var res = await RoleManager.AddClaimToRole(_defaultRoles[0], claimModel).ConfigureAwait(false);
                HandleResult(res, name);
            }
            _logInformation(name, "claims assigned");
        }

        private async Task AssignRolesToAdministrator()
        {
            const string name = "Role assignment";
            _logInformation(name, "Adding administrator role to user 'Administrator'");
            var version = await UserInfo.GetUserVersion(Administrator).ConfigureAwait(false);
            if (version.IsEmpty)
                Logger.LogError("{name} - {part} - Could not retrieve Administrator account version!", ServiceName, name);

            var res = await UserManager.AddUserToRole(Administrator, version, _defaultRoles[0]).ConfigureAwait(false);
            HandleResult(res, name);
            _logInformation(name, "Role added");
        }

        private async Task AddDefaultSeason()
        {
            const string name = "Seasons";
            try
            {
                _logInformation(name, "Beginning default season seeding");
                await using (var ctx = ContextFactory.CreateDbContext())
                {
                    var existingData = await ctx.Seasons.FirstOrDefaultAsync(x => x.Name.Equals(_defaultSeason.Name, StringComparison.OrdinalIgnoreCase)).ConfigureAwait(false);
                    if (existingData is not null)
                    {
                        ctx.Remove(existingData);
                        await ctx.SaveChangesAsync().ConfigureAwait(false);
                        _logInformation(name, "Old default season removed");
                    }
                }

                await using var cctx = ContextFactory.CreateDbContext();
                await cctx.AddAsync(_defaultSeason).ConfigureAwait(false);
                await cctx.SaveChangesAsync().ConfigureAwait(false);
                _logInformation(name, "Default season added");
                _logInformation(name, "Finished default season seeding");
            }
            catch (DbUpdateException)
            {
                Logger.LogError("{name} - {part} - seeding failed!", ServiceName, name);
                throw;
            }
        }

        private void HandleResult<TResult>(TResult result, string seedPartName) where TResult : Result
        {
            if (result.Conclusion || result.ErrorDescription?.ErrorType == ErrorType.Duplicated)
                return;

            seedPartName ??= "Unknown seed part";
            Logger.LogError("{name} - {part} - Error occurred during seeding data: {error}", ServiceName, seedPartName,
                result.ErrorDescription?.ToString() ?? "unknown error");

            const string text = "Error occurred during initial seeding";
            if (result.ErrorDescription?.Exception is not null)
                throw new InitialSeedException(seedPartName, text, result.ErrorDescription.Exception);
            throw new InitialSeedException(seedPartName, text);
        }
    }
}