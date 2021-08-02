using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    /// <summary>
    /// Provides abstraction over standard functionality of <see cref="DbContext"/> so the layer separation can be maintained.
    /// <para/>This interface does not provide any abstractions of <see cref="DbSet{TEntity}"/> on it's own.
    /// </summary>
    public interface IDbContext : IAsyncDisposable, IDisposable
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