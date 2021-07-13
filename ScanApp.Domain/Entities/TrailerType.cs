using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Domain.Entities
{
    /// <summary>
    /// Entity representing a single type of trailer (LKW) configuration.
    /// </summary>
    public class TrailerType
    {
        /// <summary>
        /// Database Id of this entity, primary key.
        /// </summary>
        public int Id { get; set; }

        private string _name;

        /// <summary>
        /// Gets or sets a name for this type of trailer (example: 'N2 - Heavy LKW').
        /// </summary>
        /// <value>A <see cref="string"/> representing name of trailer type.</value>
        /// <exception cref="ArgumentException">Given name was <see langword="null"/> or contained only whitespaces.</exception>
        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value) is false ? value : throw new ArgumentException("Name must contain something that is not whitespace.", nameof(Name));
        }

        private float _maxVolume;

        /// <summary>
        /// Gets or sets the maximum allowed volume available for this trailer to hold.
        /// </summary>
        /// <value><see cref="float"/> value representing maximum available volume in cubic meters if set, otherwise 0.</value>
        /// <exception cref="ArgumentException">Given volume was less then 0.</exception>
        public float MaxVolume
        {
            get => _maxVolume;
            set => _maxVolume = value >= 0 ? value : throw new ArgumentException("Volume has to be >= 0", nameof(MaxVolume));
        }

        private float _maxWeight;

        /// <summary>
        /// Gets or sets the maximum allowed transport weight of this trailer.
        /// </summary>
        /// <value><see cref="float"/> value representing maximum available weight in kilograms if set, otherwise 0.</value>
        /// <exception cref="ArgumentException">Given weight was less then 0.</exception>
        public float MaxWeight
        {
            get => _maxWeight;
            set => _maxWeight = value >= 0 ? value : throw new ArgumentException("Maximum weight has to be >= 0.", nameof(MaxWeight));
        }

        private TimeSpan _loadingTime;

        /// <summary>
        /// Gets or sets the average loading time for this type of trailer.
        /// </summary>
        /// <value><see cref="TimeSpan"/> value representing average loading time if set, otherwise <see cref="TimeSpan.Zero"/>.</value>
        /// <exception cref="ArgumentException">Given loading time was less then 0.</exception>
        public TimeSpan LoadingTime
        {
            get => _loadingTime;
            set => _loadingTime = value >= TimeSpan.Zero ? value : throw new ArgumentException("Loading time has to be >= 0.", nameof(LoadingTime));
        }

        private TimeSpan _unloadingTime;

        /// <summary>
        /// Gets or sets the average unloading time for this type of trailer.
        /// </summary>
        /// <value><see cref="TimeSpan"/> value representing average unloading time if set, otherwise <see cref="TimeSpan.Zero"/>.</value>
        /// <exception cref="ArgumentException">Given unloading time was less then 0.</exception>
        public TimeSpan UnloadingTime
        {
            get => _unloadingTime;
            set => _unloadingTime = value >= TimeSpan.Zero ? value : throw new ArgumentException("Unloading time has to be >= 0.", nameof(UnloadingTime));
        }

        private Version _version = Version.Empty();

        /// <summary>
        /// Gets or sets entity Version (representation of RowVersion).
        /// </summary>
        /// <value>Version of this entity, by default an empty <see cref="Version"/>.</value>
        /// <exception cref="ArgumentNullException">Given <see cref="ValueObjects.Version"/> was <see langword="null"/>.</exception>
        public Version Version
        {
            get => _version;
            set => _version = value ?? throw new ArgumentNullException(nameof(ValueObjects.Version), "Version cannot be null - use 'Version.Empty' instead.");
        }

        /// <summary>
        /// Creates new instance of <see cref="TrailerType"/>.
        /// </summary>
        /// <param name="name">Name of type of the trailer type being created.</param>
        /// <exception cref="ArgumentException">Given <paramref name="name"/> was <see langword="null"/> or contained only whitespaces.</exception>
        public TrailerType(string name)
        {
            Name = name;
        }
    }
}