using System;
using System.Collections.Generic;
using ScanApp.Domain.Common;

namespace ScanApp.Domain.ValueObjects
{
    public sealed class ConcurrencyStamp : ValueObject
    {
        public static ConcurrencyStamp Create(string stamp) => new(stamp);

        public static ConcurrencyStamp None() => new(null);

        private ConcurrencyStamp(string value)
        {
            if (value is not null && string.IsNullOrWhiteSpace(value))
                throw new FormatException($"{nameof(value)} must be either null or contain actual symbols");

            Value = value;
        }

        public string Value { get; }

        public override string ToString() => Value;

        public static implicit operator string(ConcurrencyStamp stamp) => stamp.Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}