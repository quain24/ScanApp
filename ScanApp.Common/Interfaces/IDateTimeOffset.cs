using System;

namespace ScanApp.Common.Interfaces
{
    /// <summary>
    /// Provides a way to replace hard-coded calls for <see cref="DateTimeOffset.Now"/> and similar for more testable design.
    /// <para><strong>Always</strong> use this interface instead of hard-coding date-time-offset calls.</para>
    /// </summary>
    public interface IDateTimeOffset
    {
        ///<inheritdoc cref="DateTimeOffset.Now"/>
        DateTimeOffset Now => DateTimeOffset.Now;

        ///<inheritdoc cref="DateTimeOffset.UtcNow"/>
        DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}