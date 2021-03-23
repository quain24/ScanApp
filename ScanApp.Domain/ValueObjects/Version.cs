using ScanApp.Domain.Common;
using System;
using System.Collections.Generic;

namespace ScanApp.Domain.ValueObjects
{
    public sealed class Version : ValueObject
    {
        public static Version Create(string stamp) => new(stamp);

        public static Version Empty() => new();

        private Version(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value), $"Do not create {nameof(Version)} with NULL value. To create empty {nameof(Version)} use {nameof(Version)}.Empty()");

            if (string.IsNullOrWhiteSpace(value))
                throw new FormatException($"{nameof(value)} must contain a value other than just whitespaces. For empty version use {nameof(Version)}.Empty()");

            Value = value;
        }

        private Version()
        {
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