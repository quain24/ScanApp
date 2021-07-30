using ScanApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace ScanApp.Domain.Entities
{
    public class DeparturePlan : VersionedEntity
    {
        public int Id { get; set; }

        public Depot Depots { get; set; }

        public List<Season> Seasons { get; private set; } = new();

        public TrailerType TrailerType { get; set; }

        public Gate Gate { get; set; }

        public DayAndTime LoadingStart { get; set; }
        public TimeSpan LoadingDuration { get; set; }

        public DayAndTime ArrivalTimeAtDepot { get; set; }

        //todo Temporary - placeholder for mode entity
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