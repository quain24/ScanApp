using ScanApp.Common.Interfaces;
using System;

namespace ScanApp.Common.Services
{
    /// <summary>
    /// Provides a way to replace hard-coded calls for <see cref="DateTimeOffset.Now"/> and similar for more testable design.
    /// <para><strong>Always</strong> use this interface instead of hard-coding date-time-offset calls.</para>
    /// </summary>
    public class DateTimeOffsetService : IDateTimeOffset
    {
        ///<inheritdoc cref="DateTimeOffset.Now"/>
        public DateTimeOffset Now => DateTimeOffset.Now;

        ///<inheritdoc cref="DateTimeOffset.UtcNow"/>
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}