namespace ScanApp.Application.SpareParts.Commands.CreateSpareParts
{
    public class SparePartModel
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public string SourceArticleId { get; set; }
        public string SparePartStoragePlaceId { get; set; }

        public SparePartModel(string name, int amount, string sourceArticleId, string sparePartStoragePlaceId)
        {
            Name = name;
            Amount = amount;
            SourceArticleId = sourceArticleId;
            SparePartStoragePlaceId = sparePartStoragePlaceId;
        }
    }
}