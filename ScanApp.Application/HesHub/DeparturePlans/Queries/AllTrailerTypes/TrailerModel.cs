using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.AllTrailerTypes
{
    public class TrailerModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Version Version { get; set; }
    }
}