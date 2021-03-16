using System;

namespace ScanApp.Domain.Entities
{
    public class Claim
    {
        public int Id { get; set; }
        public string Type { get; private set; }
        public string Value { get; private set; }

        public Claim(int id, string type, string value) : this(type, value)
        {
            Id = id;
        }

        public Claim(string type, string value)
        {
            ChangeType(type);
            ChangeValue(value);
        }

        public void ChangeType(string type)
        {
            Type = string.IsNullOrWhiteSpace(type)
                ? throw new ArgumentOutOfRangeException(nameof(type), "Claims type cannot be neither null nor contain only whitespaces")
                : type;
        }

        public void ChangeValue(string value)
        {
            Value = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentOutOfRangeException(nameof(value), "Claims value cannot be neither null nor contain only whitespaces")
                : value;
        }
    }
}