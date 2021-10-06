namespace ScanApp.Pages.HesHub.DeparturePlans
{
    public class CompositeResource
    {
        public int Id { get; set; }
        public int? GateId { get; set; }
        public string GateName { get; set; }
        public string SeasonId { get; set; }
        public string SeasonName { get; set; }
        public string Color { get; set; } = "#56ca85";
    }
}