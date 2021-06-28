using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using ScanApp.Application.Common.Entities;
using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IApplicationDbContext : IAsyncDisposable, IDisposable
    {
        /// <inheritdoc cref="DbContext.SaveChangesFailed"/>
        event EventHandler<SaveChangesFailedEventArgs> SaveChangesFailed;

        /// <inheritdoc cref="DbContext.SavedChanges"/>
        event EventHandler<SavedChangesEventArgs> SavedChanges;

        /// <inheritdoc cref="DbContext.SavingChanges"/>
        event EventHandler<SavingChangesEventArgs> SavingChanges;

        /// <inheritdoc cref="DbContext.ChangeTracker"/>
        ChangeTracker ChangeTracker { get; }

        /// <inheritdoc cref="DbContext.ContextId"/>
        DbContextId ContextId { get; }

        /// <inheritdoc cref="DbContext.Database"/>
        DatabaseFacade Database { get; }

        /// <inheritdoc cref="DbContext.Model"/>
        IModel Model { get; }

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

        DbSet<HesDepot> HesDepots { get; set; }

        /// <inheritdoc cref="DbContext.Add{TEntity}"/>
        EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;

        /// <inheritdoc cref="DbContext.Add(object)"/>
        EntityEntry Add(object entity);

        /// <inheritdoc cref="DbContext.AddAsync{TEntity}"/>
        ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;

        /// <inheritdoc cref="DbContext.AddAsync(object, CancellationToken)"/>
        ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="DbContext.AddRange(object[])"/>
        void AddRange(params object[] entities);

        /// <inheritdoc cref="DbContext.AddRange(IEnumerable{object})"/>
        void AddRange(IEnumerable<object> entities);

        /// <inheritdoc cref="DbContext.AddRangeAsync(object[])"/>
        Task AddRangeAsync(params object[] entities);

        /// <inheritdoc cref="DbContext.AddRangeAsync(IEnumerable{object}, CancellationToken)"/>
        Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="DbContext.Attach{TEntity}"/>
        EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;

        /// <inheritdoc cref="DbContext.Attach(object)"/>
        EntityEntry Attach(object entity);

        /// <inheritdoc cref="DbContext.AttachRange(object[])"/>
        void AttachRange(params object[] entities);

        /// <inheritdoc cref="DbContext.AttachRange(IEnumerable{object})"/>
        void AttachRange(IEnumerable<object> entities);

        /// <inheritdoc cref="DbContext.Entry{TEntity}"/>
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        /// <inheritdoc cref="DbContext.Entry(object)"/>
        EntityEntry Entry(object entity);

        /// <inheritdoc cref="DbContext.Equals(object)"/>
        bool Equals(object obj);

        /// <inheritdoc cref="DbContext.Find(Type, object[])"/>
        object Find(Type entityType, params object[] keyValues);

        /// <inheritdoc cref="DbContext.Find{TEntity}"/>
        TEntity Find<TEntity>(params object[] keyValues) where TEntity : class;

        /// <inheritdoc cref="DbContext.FindAsync(Type, object[])"/>
        ValueTask<object> FindAsync(Type entityType, params object[] keyValues);

        /// <inheritdoc cref="DbContext.FindAsync(Type, object[], CancellationToken)"/>
        ValueTask<object> FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken);

        /// <inheritdoc cref="DbContext.FindAsync{TEntity}(object[])"/>
        ValueTask<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class;

        /// <inheritdoc cref="DbContext.FindAsync{TEntity}(object[], CancellationToken)"/>
        ValueTask<TEntity> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken) where TEntity : class;

        /// <inheritdoc cref="DbContext.FromExpression{TResult}"/>
        IQueryable<TResult> FromExpression<TResult>(Expression<Func<IQueryable<TResult>>> expression);

        /// <inheritdoc cref="DbContext.GetHashCode"/>
        int GetHashCode();

        /// <inheritdoc cref="DbContext.Remove{TEntity}"/>
        EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;

        /// <inheritdoc cref="DbContext.Remove(object)"/>
        EntityEntry Remove(object entity);

        /// <inheritdoc cref="DbContext.RemoveRange(object[])"/>
        void RemoveRange(params object[] entities);

        /// <inheritdoc cref="DbContext.RemoveRange(IEnumerable{object})"/>
        void RemoveRange(IEnumerable<object> entities);

        /// <inheritdoc cref="DbContext.SaveChanges()"/>
        int SaveChanges();

        /// <inheritdoc cref="DbContext.SaveChanges(bool)"/>
        int SaveChanges(bool acceptAllChangesOnSuccess);

        /// <inheritdoc cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc cref="DbContext.SaveChangesAsync(bool, CancellationToken)"/>
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="DbContext.Set{TEntity}()"/>
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        /// <inheritdoc cref="DbContext.Set{TEntity}(string)"/>
        DbSet<TEntity> Set<TEntity>(string name) where TEntity : class;

        /// <inheritdoc cref="DbContext.ToString"/>
        string ToString();

        /// <inheritdoc cref="DbContext.Update{TEntity}"/>
        EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;

        /// <inheritdoc cref="DbContext.Update(object)"/>
        EntityEntry Update(object entity);

        /// <inheritdoc cref="DbContext.UpdateRange(object[])"/>
        void UpdateRange(params object[] entities);

        /// <inheritdoc cref="DbContext.UpdateRange(IEnumerable{object})"/>
        void UpdateRange(IEnumerable<object> entities);
    }
}