using Microsoft.EntityFrameworkCore;

namespace ScanApp.Application.Common.Interfaces
{
    /// <summary>
    /// Represents a wrapper around <see cref="IDbContextFactory{TContext}"/> that enables usage of factory inside application layer.
    /// </summary>
    /// <remarks>
    /// <strong>Caller is responsible for manual disposing of instances of <see cref="IApplicationDbContext"/> created by this factory.</strong>
    /// </remarks>
    public interface IContextFactory
    {
        /// <summary>
        /// Create new instance of <see cref="IApplicationDbContext"/>.
        /// </summary>
        /// <remarks>
        /// <strong>Caller is responsible for manual disposing of instances of <see cref="IApplicationDbContext"/> created by this factory.</strong>
        /// </remarks>
        /// <returns>New instance of <see cref="IApplicationDbContext"/>.</returns>
        IApplicationDbContext CreateDbContext();
    }
}