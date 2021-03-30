using System;
using ScanApp.Common.Interfaces;

namespace ScanApp.Common.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Today => DateTime.Today;
    }
}