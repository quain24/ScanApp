using System;

namespace ScanApp.Common.Interfaces
{
    public interface IDateTime
    {
        DateTime Now { get; }
        DateTime Today { get; }
        DateTime UtcNow { get; }
    }
}