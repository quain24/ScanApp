using ScanApp.Common.Interfaces;
using System;

namespace ScanApp.Common.Services
{
    /// <summary>
    /// Provides basic implementation of <see cref="IDateTime"/> interface.
    /// </summary>
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Today => DateTime.Today;
    }
}