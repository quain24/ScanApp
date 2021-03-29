namespace ScanApp.Application.SpareParts.Commands.CreateSpareParts
{
    public class SparePartModel
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public string SourceArticleId { get; set; }
        public string StoragePlaceId { get; set; }

        public SparePartModel(string name, int amount, string sourceArticleId, string storagePlaceId)
        {
            Name = name;
            Amount = amount;
            SourceArticleId = sourceArticleId;
            StoragePlaceId = storagePlaceId;
        }
    }
}