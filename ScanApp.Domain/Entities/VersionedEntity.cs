using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Domain.Entities
{
    public abstract class VersionedEntity
    {
        /// <summary>
        /// Gets or sets entity's Version (representation of RowVersion).
        /// </summary>
        /// <value>Version of this entity, by default <see cref="ValueObjects.Version.Empty"/>.</value>
        /// <exception cref="ArgumentNullException">Given <see cref="ValueObjects.Version"/> was <see langword="null"/>.</exception>
        public virtual Version Version
        {
            get => _version;
            set => _version = value ?? throw new ArgumentNullException(nameof(Version),
                $"Version cannot be null - use '{nameof(ValueObjects.Version)}.{nameof(Version.Empty)}' instead.");
        }

        private Version _version = Version.Empty;
    }
}