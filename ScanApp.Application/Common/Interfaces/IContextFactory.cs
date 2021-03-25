using Microsoft.EntityFrameworkCore;

namespace ScanApp.Application.Common.Interfaces
{
    /// <summary>
    /// Wrapper around <see cref="IDbContextFactory{TContext}"/> that enables usage of factory inside application layer<br/>
    /// Caller is responsible for manual disposing of instances of <see cref="IApplicationDbContext"/> created by this factory
    /// </summary>
    public interface IContextFactory
    {
        /// <summary>
        /// Create new instance of <see cref="IApplicationDbContext"/>
        /// <para>Caller is responsible for disposing of this new instance, it wont be done by DI</para>
        /// </summary>
        /// <returns>New instance of <see cref="IApplicationDbContext"/> that will require manual disposing</returns>
        IApplicationDbContext CreateDbContext();
    }
}