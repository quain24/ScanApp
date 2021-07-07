using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScanApp.Components.Common.ScanAppTable.Options;
using ScanApp.Components.Common.ScanAppTable.Utilities;

namespace ScanApp.Components.Common.ScanAppTable.Extensions
{
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Gets value based on a given <paramref name="propertyInfo"/> from a given <paramref name="obj"/>.
        /// Needs <paramref name="columnConfiguration"/> of a column associated with the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <param name="obj"></param>
        /// <param name="columnConfiguration"></param>
        /// <returns></returns>
        public static object? GetValue<T>(this PropertyInfo propertyInfo, object? obj, ColumnConfiguration<T> columnConfiguration)
        {
            if (columnConfiguration.IsNested)
            {
                return NestedPropertyHandler.GetNestedPropertyValue(obj, columnConfiguration.PropertyFullName);
            }

            if (propertyInfo.PropertyType.IsDateTime())
            {
                try
                {
                    var date = (DateTime)propertyInfo.GetValue(obj);
                    return GetDateForm(columnConfiguration.DateTimeFormat, date);
                }
                catch
                {
                    return propertyInfo.GetValue(obj);
                }
            }
            
            return propertyInfo.GetValue(obj);
        }

        /// <summary>
        /// Gets a DateTime value based on a given <paramref name="propertyInfo"/> from a given <paramref name="obj"/>.
        /// Needs <paramref name="columnConfiguration"/> of a column associated with the property.
        /// The method overrides any <see cref="DateTimeFormat"/> configuration, always giving a full DateTime object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <param name="obj"></param>
        /// <param name="columnConfiguration"></param>
        /// <returns></returns>
        public static object? GetDateTimeValue<T>(this PropertyInfo propertyInfo, object? obj,
            ColumnConfiguration<T> columnConfiguration)
        {
            if (!columnConfiguration.PropertyType.IsDateTime())
            {
                throw new ArgumentException( nameof(GetDateTimeValue) + " method should only be used with DateTime types!",
                    nameof(columnConfiguration.PropertyType));
            }
            if (columnConfiguration.IsNested)
            {
                return NestedPropertyHandler.GetNestedPropertyValue(obj, columnConfiguration.PropertyFullName);
            }
            return propertyInfo.GetValue(obj);
        }

        private static object GetDateForm(DateTimeFormat.Show dateShow, DateTime date) =>
            dateShow switch
            {
                DateTimeFormat.Show.DateOnly => date.ToShortDateString(),
                DateTimeFormat.Show.DateAndTime => date.ToShortDateString() + " " + date.ToShortTimeString(),
                DateTimeFormat.Show.DayOfWeek => date.DayOfWeek,
                DateTimeFormat.Show.DayOnly => date.Day,
                DateTimeFormat.Show.MonthOnly => date.Month,
                DateTimeFormat.Show.YearOnly => date.Year,
                DateTimeFormat.Show.TimeOnly => date.ToShortTimeString(),
                DateTimeFormat.Show.TimeWithSeconds => date.ToLongTimeString(),
                _ => date
            };
    }
}
