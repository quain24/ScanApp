namespace ScanApp.Application.SpareParts.Queries.AllSparePartTypes
{
    /// <summary>
    /// Represents simplified Spare part type data set used as return type for queries.
    /// </summary>
    public class SparePartTypeModel
    {
        /// <summary>
        /// Gets Name of Spare Part Type.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates new instance of <see cref="SparePartTypeModel"/> with given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of Spare Part Type.</param>
        public SparePartTypeModel(string name)
        {
            Name = name;
        }
    }
}