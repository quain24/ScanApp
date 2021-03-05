using ScanApp.Domain.Common;
using System;
using System.Collections.Generic;

namespace ScanApp.Domain.ValueObjects
{
    public sealed class Version : ValueObject
    {
        public static Version Create(string stamp) => new(stamp);

        public static Version Empty() => new(null);

        private Version(string value)
        {
            if (value is not null && string.IsNullOrWhiteSpace(value))
                throw new FormatException($"{nameof(value)} must contain a value other than just whitespaces. For empty version use {nameof(Version)}.Empty()");

            Value = value;
        }

        public string Value { get; }

        public bool IsEmpty => Value is null;

        public override string ToString() => Value;

        public static implicit operator string(Version stamp) => stamp.Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}