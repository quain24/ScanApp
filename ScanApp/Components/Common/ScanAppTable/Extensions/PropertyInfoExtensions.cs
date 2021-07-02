using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static object? GetValue<T>(this PropertyInfo propertyInfo, object? obj, ColumnConfig<T> columnConfig)
        {
            if (columnConfig.IsNested)
            {
                return GetPropertyValue(obj, columnConfig.PropertyFullName);
            }

            if (propertyInfo.PropertyType.IsDateTime())
            {
                try
                {
                    var date = (DateTime)propertyInfo.GetValue(obj);
                    return GetDateForm(columnConfig.DateTimeFormat, date);
                }
                catch
                {
                    return propertyInfo.GetValue(obj);
                }
            }
            
            return propertyInfo.GetValue(obj);
        }

        public static object? GetDateTimeValue<T>(this PropertyInfo propertyInfo, object? obj,
            ColumnConfig<T> columnConfig)
        {
            if (!columnConfig.PropertyType.IsDateTime())
            {
                throw new ArgumentException( nameof(GetDateTimeValue) + " method should only be used with DateTime types!",
                    nameof(columnConfig.PropertyType));
            }
            if (columnConfig.IsNested)
            {
                return GetPropertyValue(obj, columnConfig.PropertyFullName);
            }
            return propertyInfo.GetValue(obj);
        }

        public static void SetPropertyValue(this PropertyInfo propertyInfo, object target, string propName, object value)
        {
            var properties = propName.Split('.');

            for (int i=0; i < (properties.Length - 1); i++)
            {
                var propertyToGet = target.GetType().GetProperty(properties[i]);
                var property_value = propertyToGet.GetValue(target, null);
                if (property_value == null)
                {
                    if (propertyToGet.PropertyType.IsClass)
                    {
                        property_value = Activator.CreateInstance(propertyToGet.PropertyType);
                        propertyToGet.SetValue(target, property_value);
                    }
                }
                target = property_value;
            }

            var propertyToSet = target.GetType().GetProperty(properties.Last());
            propertyToSet.SetValue(target, value);
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

        private static object GetPropertyValue(object src, string propName)
        {
            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains(".")) //complex type nested
            {
                var temp = propName.Split(new char[] {'.'}, 2);
                return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.GetValue(src, null) : null;
            }
        }
    }
}
