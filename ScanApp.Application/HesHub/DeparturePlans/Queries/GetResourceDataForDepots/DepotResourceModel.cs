using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.GetResourceDataForDepots
{
    public class DepotResourceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Version Version { get; set; }
    }
}