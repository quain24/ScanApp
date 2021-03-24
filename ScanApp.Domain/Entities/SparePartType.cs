using System;

namespace ScanApp.Domain.Entities
{
    public class SparePartType
    {
        public SparePartType(string name)
        {
            Name = name;
        }

        private string _name;

        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentOutOfRangeException(nameof(Name),
                    "Name of spare part cannot be null, empty or just whitespace")
                : value;
        }
    }
}