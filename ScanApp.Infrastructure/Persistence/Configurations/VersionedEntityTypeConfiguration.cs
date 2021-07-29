using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Provides default configuration for entities that are derived from <see cref="VersionedEntity"/><br/>
    /// and therefore are using <c>RowVersion</c> mechanism.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity being configured, which must derive from <see cref="VersionedEntity"/>.</typeparam>
    public abstract class VersionedEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : VersionedEntity
    {
        /// <summary>
        /// When overridden, configures given <typeparamref name="TEntity"/>.<para/>
        /// Base implementation of this method <b>must</b> be called when overriding for configuration of <see cref="VersionedEntity"/> to take effect.
        /// </summary>
        /// <param name="builder"></param>
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(e => e.Version)
                .HasComment("This Row version is converted to 'Version' object in ScanApp")
                .IsRowVersion()
                .HasConversion(new VersionConverter());
        }
    }
}