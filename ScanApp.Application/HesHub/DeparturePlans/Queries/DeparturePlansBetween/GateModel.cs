using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.DeparturePlansBetween
{
    public class GateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Version Version { get; set; }
    }
}