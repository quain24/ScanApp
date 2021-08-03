using ScanApp.Domain.Common;
using System;
using System.Collections.Generic;

namespace ScanApp.Domain.ValueObjects
{
    public sealed class Occurrence : ValueObject
    {
        public Recurrence? Type { get; set; }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            throw new NotImplementedException();
        }
    }
}

public enum Recurrence
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}