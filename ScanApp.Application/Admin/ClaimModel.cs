using System;

namespace ScanApp.Application.Admin
{
    public class ClaimModel : IEquatable<ClaimModel>
    {
        public string Type { get; }
        public string Value { get; }

        public ClaimModel(string type, string value)
        {
            Type = string.IsNullOrWhiteSpace(type)
                ? throw new ArgumentOutOfRangeException(nameof(type), "Type must not be null, empty or whitespace-only")
                : type;

            Value = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentOutOfRangeException(nameof(value), "Value must not be empty or contain only whitespaces")
                : value;
        }

        public bool Equals(ClaimModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((ClaimModel)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
        }
    }
}