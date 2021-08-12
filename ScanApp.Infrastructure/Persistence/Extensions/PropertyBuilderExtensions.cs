using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScanApp.Infrastructure.Persistence.Extensions
{
    public static class PropertyBuilderExtensions
    {
        private static DateTime FromCodeToData(DateTime fromCode, string name)
            => fromCode.Kind == DateTimeKind.Utc ? fromCode : throw new InvalidOperationException($"Column {name} only accepts UTC date-time values");

        private static DateTime FromDataToCode(DateTime fromData)
            => fromData.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fromData, DateTimeKind.Utc) : fromData.ToUniversalTime();

        /// <summary>
        /// <see cref="DateTime"/> will be stored as UTC (source data must be stored with <see cref="DateTimeKind.Utc"/>) and returned as <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/>.
        /// </summary>
        /// <param name="property">Property being configured.</param>
        /// <returns>This instance of builder for further configuration.</returns>
        public static PropertyBuilder<DateTime?> UsesUtc(this PropertyBuilder<DateTime?> property)
        {
            var name = property.Metadata.Name;
            return property.HasConversion<DateTime?>(
                fromCode => fromCode != null ? FromCodeToData(fromCode.Value, name) : default,
                fromData => fromData != null ? FromDataToCode(fromData.Value) : default
            );
        }

        /// <summary>
        /// <see cref="DateTime"/> will be stored as UTC (source data must be stored with <see cref="DateTimeKind.Utc"/>) and returned as <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/>.
        /// </summary>
        /// <param name="property">Property being configured.</param>
        /// <returns>This instance of builder for further configuration.</returns>
        public static PropertyBuilder<DateTime> UsesUtc(this PropertyBuilder<DateTime> property)
        {
            var name = property.Metadata.Name;
            return property.HasConversion(fromCode => FromCodeToData(fromCode, name),
                fromData => FromDataToCode(fromData));
        }
    }
}