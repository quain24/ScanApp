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
    public interface IApplicationDbContext
    {
        event EventHandler<SaveChangesFailedEventArgs> SaveChangesFailed;

        event EventHandler<SavedChangesEventArgs> SavedChanges;

        event EventHandler<SavingChangesEventArgs> SavingChanges;

        ChangeTracker ChangeTracker { get; }
        DbContextId ContextId { get; }
        DatabaseFacade Database { get; }
        IModel Model { get; }
        DbSet<IdentityRoleClaim<string>> RoleClaims { get; set; }
        DbSet<IdentityRole> Roles { get; set; }
        DbSet<IdentityUserClaim<string>> UserClaims { get; set; }
        DbSet<IdentityUserLogin<string>> UserLogins { get; set; }
        DbSet<IdentityUserRole<string>> UserRoles { get; set; }
        DbSet<ApplicationUser> Users { get; set; }
        DbSet<IdentityUserToken<string>> UserTokens { get; set; }
        DbSet<Location> Locations { get; set; }
        DbSet<UserLocation> UserLocations { get; set; }
        public DbSet<Claim> ClaimsSource { get; set; }

        DbSet<SparePart> SpareParts { get; set; }
        DbSet<SparePartType> SparePartTypes { get; set; }
        DbSet<StoragePlace> StoragePlaces { get; set; }

        EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;

        EntityEntry Add(object entity);

        ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;

        ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default);

        void AddRange(params object[] entities);

        void AddRange(IEnumerable<object> entities);

        Task AddRangeAsync(params object[] entities);

        Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default);

        EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;

        EntityEntry Attach(object entity);

        void AttachRange(params object[] entities);

        void AttachRange(IEnumerable<object> entities);

        void Dispose();

        ValueTask DisposeAsync();

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        EntityEntry Entry(object entity);

        bool Equals(object obj);

        object Find(Type entityType, params object[] keyValues);

        TEntity Find<TEntity>(params object[] keyValues) where TEntity : class;

        ValueTask<object> FindAsync(Type entityType, params object[] keyValues);

        ValueTask<object> FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken);

        ValueTask<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class;

        ValueTask<TEntity> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken) where TEntity : class;

        IQueryable<TResult> FromExpression<TResult>(Expression<Func<IQueryable<TResult>>> expression);

        int GetHashCode();

        EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;

        EntityEntry Remove(object entity);

        void RemoveRange(params object[] entities);

        void RemoveRange(IEnumerable<object> entities);

        int SaveChanges();

        int SaveChanges(bool acceptAllChangesOnSuccess);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        DbSet<TEntity> Set<TEntity>(string name) where TEntity : class;

        string ToString();

        EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;

        EntityEntry Update(object entity);

        void UpdateRange(params object[] entities);

        void UpdateRange(IEnumerable<object> entities);
    }
}