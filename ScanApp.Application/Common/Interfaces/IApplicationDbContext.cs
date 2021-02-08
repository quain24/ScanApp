using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ScanApp.Application.Common.Entities;

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

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        int SaveChanges();

        ValueTask DisposeAsync();
        void Dispose();


        ChangeTracker ChangeTracker { get; }
        event EventHandler<SavingChangesEventArgs> SavingChanges;
        event EventHandler<SavedChangesEventArgs> SavedChanges;
        event EventHandler<SaveChangesFailedEventArgs> SaveChangesFailed;
    }
}