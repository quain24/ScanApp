using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Entities;
using ScanApp.Domain.Entities;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IApplicationDbContext : IDbContext
    {
        /// <summary>
        /// Gets or sets <see cref="IdentityRoleClaim{TKey}">User Role Claims</see> database table representation.
        /// <br/>Each <see cref="IdentityRoleClaim{TKey}">User Role Claim</see> stored here is accessible to all users assigned to a corresponding role.
        /// </summary>
        /// <remarks>
        /// <see cref="IdentityRoleClaim{TKey}.RoleId"/> of each entry must match one of <see cref="Roles"/> id's.<br/>
        /// <see cref="IdentityRoleClaim{TKey}.ClaimType"/> and <see cref="IdentityRoleClaim{TKey}.ClaimValue"/> of each entry
        /// must match one of <see cref="ClaimsSource"/> types and corresponding values.
        /// </remarks>
        /// <value>Set of role claims.</value>
        DbSet<IdentityRoleClaim<string>> RoleClaims { get; set; }

        /// <summary>
        /// Gets or sets <see cref="IdentityRole">User Roles</see> database table representation.
        /// </summary>
        /// <value>Set of user roles.</value>
        DbSet<IdentityRole> Roles { get; set; }

        /// <summary>
        /// Gets or sets <see cref="IdentityUserClaim{TKey}">User Claims</see> database table representation.
        /// </summary>
        /// <remarks>
        /// <see cref="IdentityUserClaim{TKey}.UserId"/> of each entry must match one of <see cref="Users"/> id's.<br/>
        /// <see cref="IdentityUserClaim{TKey}.ClaimType"/> and <see cref="IdentityUserClaim{TKey}.ClaimValue"/> of each entry must match <strong>together</strong>
        /// one of <see cref="ClaimsSource"/> type and corresponding value.
        /// </remarks>
        /// <value>Set of user claims.</value>
        DbSet<IdentityUserClaim<string>> UserClaims { get; set; }

        /// <summary>
        /// Gets or sets <see cref="IdentityUserLogin{TKey}">User Logins</see> database table representation.
        /// </summary>
        /// <value>Set of user logins.</value>
        DbSet<IdentityUserLogin<string>> UserLogins { get; set; }

        /// <summary>
        /// Gets or sets <see cref="IdentityUserRole{TKey}">User Roles</see> database table representation.
        /// </summary>
        /// <remarks>
        /// This is an intermediary table connecting <see cref="Users"/> and <see cref="Roles"/> by id's.<br/>
        /// <see cref="IdentityUserRole{TKey}.UserId"/> of each entry must match one of <see cref="Users"/> id's.<br/>
        /// <see cref="IdentityUserRole{TKey}.RoleId"/> of each entry must match one of <see cref="Roles"/> id's.
        /// </remarks>
        /// <value>Set of user roles.</value>
        DbSet<IdentityUserRole<string>> UserRoles { get; set; }

        /// <summary>
        /// Gets or sets <see cref="ApplicationUser">Users</see> database table representation.
        /// </summary>
        /// <value>Set of users.</value>
        DbSet<ApplicationUser> Users { get; set; }

        /// <summary>
        /// Gets or sets <see cref="IdentityUserToken{TKey}">User Tokens</see> database table representation.
        /// </summary>
        /// <value>Set of user tokens.</value>
        DbSet<IdentityUserToken<string>> UserTokens { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Location">Locations</see> database table representation.
        /// </summary>
        /// <value>Set of locations.</value>
        DbSet<Location> Locations { get; set; }

        /// <summary>
        /// Gets or sets <see cref="UserLocation">User Locations</see> database table representation.
        /// </summary>
        /// <remarks>
        /// This is an intermediary table connecting <see cref="Users"/> and <see cref="Locations"/> by id's.<br/>
        /// <see cref="UserLocation.UserId"/> of each entry must match one of <see cref="Users"/> id's.<br/>
        /// <see cref="UserLocation.LocationId"/> of each entry must match one of <see cref="Locations"/> id's.
        /// </remarks>
        /// <value>Set of user locations.</value>
        DbSet<UserLocation> UserLocations { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Claim">Claims source</see> database table representation.
        /// <br/>Only <see cref="Claim"/> stored in this table should be assigned to either a user or a role.
        /// </summary>
        /// <value>Set of claims.</value>
        DbSet<Claim> ClaimsSource { get; set; }

        /// <summary>
        /// Gets or sets <see cref="SparePart">Spare Parts</see> database table representation.
        /// </summary>
        /// <remarks>
        /// <see cref="SparePart.Name"/> of each entry must match one of <see cref="SparePartTypes"/> names.<br/>
        /// <see cref="SparePart.SparePartStoragePlaceId"/> of each entry must match one of <see cref="SparePartStoragePlaces"/> id's.
        /// </remarks>
        /// <value>Set of spare parts.</value>
        DbSet<SparePart> SpareParts { get; set; }

        /// <summary>
        /// Gets or sets <see cref="SparePartType">Spare Part Types</see> database table representation.
        /// <br/>Only <see cref="SparePartType.Name"/> stored in this table should used when creating / modifying one of <see cref="SparePart"/>.
        /// </summary>
        /// <value>Set of Spare Part Types.</value>
        DbSet<SparePartType> SparePartTypes { get; set; }

        /// <summary>
        /// Gets or sets <see cref="SparePartStoragePlace">Spare Part Storage Places</see> database table representation.
        /// <br/>Only <see cref="SparePartStoragePlace.Id"/> stored in this table should used when creating / modifying one of <see cref="SparePart"/>.
        /// <br/><see cref="SparePartStoragePlace.LocationId"/> of each entry must match one of <see cref="Locations"/> id's.
        /// </summary>
        /// <value>Set of Spare Part Storage Places.</value>
        DbSet<SparePartStoragePlace> SparePartStoragePlaces { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Depot">HES Depot</see> database table representation.
        /// </summary>
        /// <value>Set of HES Depots.</value>
        DbSet<Depot> Depots { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Gate">Gates</see> database table representation.
        /// </summary>
        /// <value>Set of warehouse 'gates' (loading docks).</value>
        DbSet<Gate> Gates { get; set; }

        /// <summary>
        /// Gets or sets <see cref="TrailerType">Trailer types</see> database table representation.
        /// </summary>
        /// <value>Set of Trailer types.</value>
        DbSet<TrailerType> TrailerTypes { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Season">Seasons</see> database table representation.<br/>
        /// A season in this context is a transport season, not mandatory a 'summer' or 'Christmas'.
        /// </summary>
        /// <value>Set of Seasons.</value>
        DbSet<Season> Seasons { get; set; }

        /// <summary>
        /// Gets or sets <see cref="DeparturePlan">Departure plans</see> database table representation.
        /// </summary>
        /// <value>Set of Departure plans.</value>
        DbSet<DeparturePlan> DeparturePlans { get; set; }
    }
}