using System;

namespace ScanApp.Domain.Entities
{
    public class SparePart
    {
        public SparePart(string id, string name, int amount, string sourceArticleId, string sparePartStoragePlaceId)
            : this(name, amount, sourceArticleId, sparePartStoragePlaceId)
        {
            Id = id;
        }

        public SparePart(string name, int amount, string sourceArticleId, string sparePartStoragePlaceId)
        {
            Name = name;
            Amount = amount;
            SourceArticleId = sourceArticleId;
            SparePartStoragePlaceId = sparePartStoragePlaceId;
        }

        public string Id { get; set; }

        private string _name;

        public string Name
        {
            get => _name;
            set => ChangeName(value);
        }

        private int _amount;

        public int Amount
        {
            get => _amount;
            set => ChangeAmount(value);
        }

        public string SourceArticleId { get; set; }
        public string SparePartStoragePlaceId { get; set; }

        private void ChangeName(string name)
        {
            _name = string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentOutOfRangeException(nameof(name),
                    $"Tried to create {nameof(SparePart)} using invalid {nameof(name)} parameter ({name}). It cannot be null or consist of just whitespaces")
                : name;
        }

        private void ChangeAmount(int amount)
        {
            _amount = amount <= 0 ? throw new ArgumentOutOfRangeException(nameof(amount), "Given amount must be a positive number") : amount;
        }
    }
}