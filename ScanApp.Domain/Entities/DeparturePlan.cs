using ScanApp.Domain.ValueObjects;
using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Domain.Entities
{
    public class DeparturePlan : VersionedEntity
    {
        public int Id { get; set; }
        public TrailerType TrailerType { get; set; }

        public Gate Gate { get; set; }

        public DayAndTime LoadingBeginning { get; set; }

        public DayAndTime LoadingFinish { get; set; }

        public TimeSpan LoadingDuration { get; set; }
    }
}

/* Departure plan have one or many Modes (Default, Christmas, etc) - Same plan for multiple Modes / one mode can have multiple plans = many to many
 * Departure plan must have at least one mode - make "default" non-deletable in modes table and force plan to have one.
 * Departure plan have one or many Depot assigned. Plan to depot = many to many: one depot can have many plans, one plan can have many depots
 * Combination of Mode and Depot for each departure plan must be unique?
 * -   Plan A for Default mode for Depot 56   - OK
 * -   Plan B for Default mode for Depot 56   - ERROR (?) - duplication of mode + depot, even if gate / trailer / other settings are different.
 * -   Plan A for Default mode for Depot 100  - OK (?) - plan settings are the same, just target depot is different.
 * -   Plan B for Christmas mode for Depot 56 - OK
 *
 * One departure plan -> many Depots
 * One departure plan -> many Modes
 * Depot + mode = unique combination per depot.
 */