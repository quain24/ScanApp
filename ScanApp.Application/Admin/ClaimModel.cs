using System;

namespace ScanApp.Application.Admin
{
    /// <summary>
    /// Represents user claim, typically used as a return type for <see cref="MediatR.Mediator"/> queries
    /// </summary>
    public class ClaimModel : IEquatable<ClaimModel>
    {
        /// <summary>
        /// Gets type of claim
        /// </summary>
        /// <value><see cref="string"/> containing type of claim</value>
        public string Type { get; }

        /// <summary>
        /// Gets value of claim
        /// </summary>
        /// <value><see cref="string"/> containing value of claim</value>
        public string Value { get; }

        /// <summary>
        /// Creates new instance of <see cref="ClaimModel"/>
        /// </summary>
        /// <param name="type">Type of claim to be stored</param>
        /// <param name="value">Value of the claim to be stored</param>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="type"/> is <see langword="null"/>, empty or contains just <c>whitespace</c></exception>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="value"/> is <see langword="null"/>, empty or contains just <c>whitespace</c></exception>
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