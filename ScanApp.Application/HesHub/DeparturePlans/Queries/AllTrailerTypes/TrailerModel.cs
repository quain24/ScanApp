using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.AllTrailerTypes
{
    public class TrailerModel
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public Version Version { get; init; }
    }
}