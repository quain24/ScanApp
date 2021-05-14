namespace ScanApp.Application.SpareParts.Commands.CreateSpareParts
{
    /// <summary>
    /// Represents set of data used for creation of new Spare part in application's data store, typically by a command.
    /// </summary>
    public class SparePartModel
    {
        /// <summary>
        /// Gets or sets Spare part's name.
        /// </summary>
        /// <value>Name of Spare Part if set, otherwise <see langword="null"/>.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets amount of created Spare Parts.
        /// </summary>
        /// <value>Number of Spare Part elements (one <see cref="SparePartModel"/> can represent any number of identical parts).</value>
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets Source article ID.<br/>
        /// An article id can be a database ID, serial number, bar-code, etc of item from which Spare Part is created.
        /// </summary>
        /// <value>Source article ID if set; Otherwise <see langword="null"/>.</value>
        public string SourceArticleId { get; set; }

        /// <summary>
        /// Gets or sets Storage Place ID for Spare Part.
        /// </summary>
        /// <value>Storage Place ID if set; Otherwise <see langword="null"/>.</value>
        public string SparePartStoragePlaceId { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="SparePartModel"/>.
        /// </summary>
        /// <param name="name">Name of spare part.</param>
        /// <param name="amount">Amount of units of this spare part.</param>
        /// <param name="sourceArticleId">ID of article from which spare parts will be created.</param>
        /// <param name="sparePartStoragePlaceId">ID of place where spare parts will be stored.</param>
        public SparePartModel(string name, int amount, string sourceArticleId, string sparePartStoragePlaceId)
        {
            Name = name;
            Amount = amount;
            SourceArticleId = sourceArticleId;
            SparePartStoragePlaceId = sparePartStoragePlaceId;
        }
    }
}