using ScanApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Domain.Entities
{
    public class DeparturePlan : VersionedEntity
    {
        public int Id { get; set; }

        public Depot Depot
        {
            get => _depot;
            set => _depot = value ?? throw new ArgumentNullException(nameof(Depot));
        }

        private Depot _depot;

        private readonly List<Season> _seasons = new();

        public IEnumerable<Season> Seasons => _seasons.AsReadOnly();

        public TrailerType TrailerType
        {
            get => _trailerType;
            set => _trailerType = value ?? throw new ArgumentNullException(nameof(TrailerType));
        }

        private TrailerType _trailerType;

        public Gate Gate
        {
            get => _gate;
            set => _gate = value ?? throw new ArgumentNullException(nameof(Gate));
        }

        private Gate _gate;

        public DayAndTime LoadingStart
        {
            get => _loadingStart;
            set => _loadingStart = value ?? throw new ArgumentNullException(nameof(LoadingStart));
        }

        private DayAndTime _loadingStart;

        public TimeSpan LoadingDuration
        {
            get => _loadingTimeDuration;
            set => _loadingTimeDuration = value < TimeSpan.Zero
                ? throw new ArgumentOutOfRangeException(nameof(LoadingDuration), "Duration must be 0 or more.")
                : value;
        }

        private TimeSpan _loadingTimeDuration;

        public DayAndTime ArrivalTimeAtDepot
        {
            get => _arrivalTimeAtDepot;
            set => _arrivalTimeAtDepot = value ?? throw new ArgumentNullException(nameof(ArrivalTimeAtDepot));
        }

        private DayAndTime _arrivalTimeAtDepot;

        // For Ef Core compliance
        private DeparturePlan()
        {
        }

        public DeparturePlan(Depot depot,
            Season season,
            Gate gate,
            TrailerType trailerType,
            DayAndTime loadingStart,
            TimeSpan loadingDuration,
            DayAndTime arrivalTimeAtDepot)
        {
            AssignToSeason(season);
            Depot = depot;
            TrailerType = trailerType;
            Gate = gate;
            LoadingStart = loadingStart;
            LoadingDuration = loadingDuration;
            ArrivalTimeAtDepot = arrivalTimeAtDepot;
        }

        public void AssignToSeason(Season season)
        {
            if (season is null)
                throw new ArgumentNullException(nameof(season));
            if (Seasons.Any(x => x.Name.Equals(season.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"{nameof(DeparturePlan)} already has a {nameof(Season)} named {season.Name}.");
            _seasons.Add(season);
            RemoveFromSeason(null);
        }

        public void RemoveFromSeason(Season season)
        {
            if (season is null)
                throw new ArgumentNullException(nameof(season));
            if (_seasons.Count <= 1)
                throw new ArgumentException($"Cannot remove only {nameof(Season)} from {nameof(DeparturePlan)}.");

            var index = _seasons.FindIndex(x => x.Name.Equals(season.Name, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
                throw new ArgumentException($"This {nameof(DeparturePlan)} does not have {nameof(Season)} named {season.Name} in it's collection.");
            _seasons.RemoveAt(index);
        }
    }
}

/* Departure plan have one or many Modes (Default, Christmas, etc) - Same plan for multiple Modes / one mode can have multiple plans = many to many
 * Departure plan must have at least one mode - make "default" non-deletable in modes table and force plan to have one.
 * Departure plan have one Depot assigned. Plan to depot = many to one: one depot can have many plans, one plan can have only one depot.
 * Combination of Mode and Depot for each departure plan must be unique.
 * -   Plan A for Default mode for Depot 56   - OK
 * -   Plan B for Default mode for Depot 56   - OK    - Mode (like Christmas) can have multiple departures to single depot.
 * -   Plan A for Default mode for Depot 100  - ERROR - Single plan is only for single depot - otherwise 2 depots would try to load at the same time on the same gate
 * -   Plan B for Christmas mode for Depot 56 - OK
 *
 *  >>>--- Detect collision Gate / time For plans in one mode ---<<<
 *
 * One departure plan -> many Depots
 * One departure plan -> many Modes
 * Depot + mode = unique combination per depot.
 */