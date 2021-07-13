using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Domain.Entities
{
    /// <summary>
    /// Represents sing gate / loading bay inside the warehouse.
    /// </summary>
    public class Gate
    {
        /// <summary>
        /// Database Id of this entity, primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets gate number (name).
        /// </summary>
        /// <value>Gate name / number.</value>
        public int Number { get; set; }

        private TrafficDirection _direction;
        /// <summary>
        /// Gets or sets gate traffic flow direction. Indicates whether this gate handles incoming, outgoing or both way traffic.
        /// </summary>
        /// <value>Gate's traffic direction. If not set by the user it will be <see cref="TrafficDirection.Incoming"/>.</value>
        public TrafficDirection Direction
        {
            get => _direction;
            set => _direction = Enum.IsDefined(typeof(TrafficDirection), value)
                ? value
                : throw new ArgumentOutOfRangeException(nameof(Direction), value,
                    $"Given {nameof(Direction)} value is not defined in {nameof(TrafficDirection)} enum.");
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Gate"/> handles incoming traffic.
        /// </summary>
        /// <value><see langword="true"/> if direction of this gate is set to either <see cref="TrafficDirection.Incoming"/>
        /// or <see cref="TrafficDirection.BiDirectional"/>, otherwise <see langword="false"/></value>
        public bool IsIncoming => Direction is TrafficDirection.Incoming or TrafficDirection.BiDirectional;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Gate"/> handles outgoing traffic.
        /// </summary>
        /// <value><see langword="true"/> if direction of this gate is set to either <see cref="TrafficDirection.Outgoing"/>
        /// or <see cref="TrafficDirection.BiDirectional"/>, otherwise <see langword="false"/>.</value>
        public bool IsOutgoing => Direction is TrafficDirection.Outgoing or TrafficDirection.BiDirectional;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Gate"/> handles incoming and outgoing traffic.
        /// </summary>
        /// <value><see langword="true"/> if direction of this gate is set to <see cref="TrafficDirection.BiDirectional"/>,
        /// otherwise <see langword="false"/>.</value>
        public bool IsBidirectional => Direction is TrafficDirection.BiDirectional;

        /// <summary>
        /// Creates new instance of <see cref="Gate"/>.
        /// </summary>
        /// <param name="number">Gate / loading bay assigned number.</param>
        /// <param name="direction">Direction of traffic for this gate.</param>
        public Gate(int number, TrafficDirection direction)
        {
            Number = number;
            Direction = direction;
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
        /// Traffic direction for a single gate / loading bay.
        /// </summary>
        public enum TrafficDirection
        {
            Incoming = 0,
            Outgoing,
            BiDirectional
        }
    }
}