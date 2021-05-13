namespace ScanApp.Application.SpareParts
{
    /// <summary>
    /// Represents a storage place specific for Spare parts.
    /// </summary>
    public class RepairWorkshopModel
    {
        /// <summary>
        /// Gets ID of this storage place.
        /// </summary>
        /// <value>ID if set; Otherwise <see langword="null"/>.</value>
        public string Id { get; init; }

        /// <summary>
        /// Gets number of this storage place.
        /// </summary>
        /// <remarks>In this case a 'number' often can be used interchangeably with 'name'.</remarks>
        /// <value>Number of this storage place if set; Otherwise <see langword="null"/>.</value>
        public string Number { get; init; }
    }
}