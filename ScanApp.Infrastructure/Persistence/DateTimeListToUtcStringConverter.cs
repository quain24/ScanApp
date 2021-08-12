using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Infrastructure.Persistence
{
    /// <summary>
    /// Provides a way to store collection of <see cref="DateTime"/> objects in single database table.<br/>
    /// Data will be stored in UTC time format <c>(example: 2008-10-31T17:04:32)</c> as comma separated string.<para/>
    /// Dates which <see cref="DateTime.Kind"/> other than <see cref="DateTimeKind.Utc"/> will cause <see cref="InvalidOperationException"/>.<para/>
    /// Values returned from database will be converted to UTC.
    /// </summary>
    public class DateTimeListToUtcStringConverter : ValueConverter<IEnumerable<DateTime>, string>
    {
        /// <summary>
        /// Creates new instance of <see cref="DateTimeListToUtcStringConverter"/>.
        /// </summary>
        public DateTimeListToUtcStringConverter() : base(data => FromCode(data), data => FromData(data))
        {
        }

        private static string FromCode(IEnumerable<DateTime> data) =>
            string.Join(",", data.Select(x => x.Kind == DateTimeKind.Utc
                ? x.ToString("s")
                : throw new InvalidOperationException(
                    $"All dates being stored must be in UTC format ({nameof(DateTimeKind)} should be set to {nameof(DateTimeKind.Utc)})")));

        private static IEnumerable<DateTime> FromData(string data) =>
            data.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => DateTime.SpecifyKind(DateTime.Parse(x), DateTimeKind.Utc))
                .ToList();
    }
}