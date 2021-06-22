using System;
using System.Reflection;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static object? GetValue(this PropertyInfo propertyInfo, object? obj, DateTimeFormat.Show dateTimeFormat)
        {
            if (propertyInfo.PropertyType.IsDateTime())
            {
                try
                {
                    var date = (DateTime)propertyInfo.GetValue(obj);
                    return GetDateForm(dateTimeFormat, date);
                }
                catch
                {
                    return propertyInfo.GetValue(obj);
                }
            }
            return propertyInfo.GetValue(obj);
        }

        private static object GetDateForm(DateTimeFormat.Show dateShow, DateTime date) =>
            dateShow switch
            {
                DateTimeFormat.Show.DateOnly => date.Day + "/" + date.Month + "/" + date.Year,
                DateTimeFormat.Show.DateAndTime => date,
                DateTimeFormat.Show.DayOfWeek => date.DayOfWeek,
                DateTimeFormat.Show.DayOnly => date.Day,
                DateTimeFormat.Show.MonthOnly => date.Month,
                DateTimeFormat.Show.YearOnly => date.Year,
                DateTimeFormat.Show.TimeOnly => date.Hour + ":" + date.Minute,
                DateTimeFormat.Show.TimeWithSeconds => date.Hour + ":" + date.Minute + ":" + date.Second,
                _ => date
            };
    }
}
