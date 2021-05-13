using System;

namespace ScanApp.Common.Interfaces
{
    /// <summary>
    /// Provides a way to replace hard-coded calls for <see cref="DateTime.Now"/> and similar for more testable design.
    /// <para><strong>Always</strong> use this interface instead of hard-coding date-time calls.</para>
    /// </summary>
    public interface IDateTime
    {
        ///<inheritdoc cref="DateTime.Now"/>
        DateTime Now { get; }

        ///<inheritdoc cref="DateTime.Today"/>
        DateTime Today { get; }

        ///<inheritdoc cref="DateTime.UtcNow"/>
        DateTime UtcNow { get; }
    }
}