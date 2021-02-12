using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ScanApp.Application.Common.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<IdentityUserRole<string>> UserRoles { get; set; }
        DbSet<IdentityRole> Roles { get; set; }
        DbSet<IdentityRoleClaim<string>> RoleClaims { get; set; }
        DbSet<ApplicationUser> Users { get; set; }
        DbSet<IdentityUserClaim<string>> UserClaims { get; set; }
        DbSet<IdentityUserLogin<string>> UserLogins { get; set; }
        DbSet<IdentityUserToken<string>> UserTokens { get; set; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        EntityEntry Add(object entity);

        EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;

        ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken);

        ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class;

        EntityEntry Attach(object entity);

        EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;

        EntityEntry Update(object entity);

        EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;

        EntityEntry Remove(object entity);

        EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;

        void AddRange(params object[] entities);

        void AddRange(IEnumerable<object> entities);

        Task AddRangeAsync(params object[] entities);

        Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken);

        void AttachRange(params object[] entities);

        void AttachRange(IEnumerable<object> entities);

        void UpdateRange(params object[] entities);

        void UpdateRange(IEnumerable<object> entities);

        void RemoveRange(params object[] entities);

        void RemoveRange(IEnumerable<object> entities);

        void Dispose();

        ValueTask DisposeAsync();

        ChangeTracker ChangeTracker { get; }

        event EventHandler<SavingChangesEventArgs> SavingChanges;

        event EventHandler<SavedChangesEventArgs> SavedChanges;

        event EventHandler<SaveChangesFailedEventArgs> SaveChangesFailed;
    }
}