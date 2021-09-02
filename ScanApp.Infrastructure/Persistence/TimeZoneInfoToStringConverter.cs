using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace ScanApp.Infrastructure.Persistence
{
    /// <summary>
    /// EF core converter to be used when saving <see cref="TimeZoneInfo"/> is needed.
    /// </summary>
    public class TimeZoneInfoToStringConverter : ValueConverter<TimeZoneInfo, string>
    {
        /// <summary>
        /// Creates new instance of <see cref="TimeZoneInfoToStringConverter"/>.
        /// </summary>
        public TimeZoneInfoToStringConverter()
            : base(data => FromCode(data), data => FromData(data), new ConverterMappingHints(256))
        {
        }

        private static string FromCode(TimeZoneInfo data) => data?.Id;

        private static TimeZoneInfo FromData(string data) =>
            data is null ? null : TimeZoneInfo.FindSystemTimeZoneById(data);
    }
}