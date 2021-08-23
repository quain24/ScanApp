using ScanApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Domain.Entities
{
    public class DeparturePlan : Occurrence<DeparturePlan>
    {
        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Name must have actual letters", nameof(Name))
                : value;
        }

        private string _name;

        public Depot Depot
        {
            get => _depot;
            set => _depot = value ?? throw new ArgumentNullException(nameof(Depot));
        }

        private Depot _depot;

        public IEnumerable<Season> Seasons => _seasons.AsReadOnly();

        private readonly List<Season> _seasons = new();

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

        public DayAndTime ArrivalTimeAtDepot
        {
            get => _arrivalTimeAtDepot;
            set => _arrivalTimeAtDepot = value ?? throw new ArgumentNullException(nameof(ArrivalTimeAtDepot));
        }

        private DayAndTime _arrivalTimeAtDepot;

        private DeparturePlan() : base()
        {
        }

        public DeparturePlan(string name,
            DateTime start,
            DateTime end,
            Depot depot,
            Season season,
            Gate gate,
            TrailerType trailerType,
            DayAndTime arrivalTimeAtDepot) : base(start, end)
        {
            Name = name;
            AssignToSeason(season);
            Depot = depot;
            TrailerType = trailerType;
            Gate = gate;
            ArrivalTimeAtDepot = arrivalTimeAtDepot;
        }

        public void AssignToSeason(Season season)
        {
            if (season is null)
                throw new ArgumentNullException(nameof(season));
            if (Seasons.Any(x => x.Name.Equals(season.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"{nameof(DeparturePlan)} already has a {nameof(Season)} named {season.Name}.");
            _seasons.Add(season);
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