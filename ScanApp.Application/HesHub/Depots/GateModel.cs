using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.Depots
{
    public class GateModel
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public Version Version { get; set; }
    }
}