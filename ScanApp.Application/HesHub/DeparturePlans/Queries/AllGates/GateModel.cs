using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.AllGates
{
    public class GateModel
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public Version Version { get; init; }
    }
}