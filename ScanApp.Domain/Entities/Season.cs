using System;
using System.Collections.Generic;

namespace ScanApp.Domain.Entities
{
    /// <summary>
    /// Represents an arbitrary time period in a year.
    /// </summary>
    public class Season : VersionedEntity
    {
        private string _name;

        /// <summary>
        /// Gets or sets unique name of this <see cref="Season"/>.
        /// </summary>
        /// <value>Name of the season as <see cref="string"/>.</value>
        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Name must contain some letters", nameof(Name))
                : value;
        }

        /// <summary>
        /// Gets starting date of this <see cref="Season"/> (UTC).
        /// </summary>
        public DateTime Start { get; private set; }

        /// <summary>
        /// Gets end date of this <see cref="Season"/> (UTC).
        /// </summary>
        public DateTime End { get; private set; }

        private readonly List<DeparturePlan> _departurePlans = new();
        public IEnumerable<DeparturePlan> DeparturePlans => _departurePlans.AsReadOnly();

        // For Ef Core compliance
        private Season()
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="Season"/>.
        /// </summary>
        /// <param name="name">Name of this season.</param>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        /// <exception cref="ArgumentException"><paramref name="startDate"/> was greater then <paramref name="endDate"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> was empty, <see langword="null"/> or contained only white-spaces.</exception>
        public Season(string name, DateTime startDate, DateTime endDate)
        {
            ChangePeriod(startDate, endDate);
            Name = name;
        }

        /// <summary>
        /// Changes this <see cref="Season"/> start and end date.
        /// </summary>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        /// <exception cref="ArgumentException"><paramref name="startDate"/> was greater then <paramref name="endDate"/>.</exception>
        public void ChangePeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate.Kind != DateTimeKind.Utc || endDate.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(startDate)} and {nameof(endDate)} both" +
                                            " must be in UTC format (DateTime Kind must be set to UTC).");
            }

            if (startDate >= endDate)
                throw new ArgumentException($"{nameof(startDate)} must be lesser or equal to {nameof(endDate)}", nameof(startDate));

            Start = startDate;
            End = endDate;
        }
    }
}