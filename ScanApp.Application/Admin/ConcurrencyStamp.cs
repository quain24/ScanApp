using System;

namespace ScanApp.Application.Admin
{
    public sealed class ConcurrencyStamp
    {
        public string Value { get; }

        public ConcurrencyStamp(string value)
        {
            if (value is not null && string.IsNullOrWhiteSpace(value))
                throw new FormatException($"{nameof(value)} must be either null or contain actual symbols");

            Value = value;
        }

        public override string ToString() => Value;
    }
}