using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries
{
    public class SeasonResourceModel
    {
        public string Name { get; set; }
        public Version Version { get; set; }
    }
}