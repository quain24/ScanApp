using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries
{
    public class SeasonResourceModel
    {
        public string Name { get; init; }
        public Version Version { get; init; }
    }
}