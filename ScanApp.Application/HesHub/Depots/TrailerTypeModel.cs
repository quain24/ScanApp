using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.Depots
{
    public class TrailerTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Version Version { get; set; }
    }
}