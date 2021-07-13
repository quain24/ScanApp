using System;
using System.Linq;

namespace ScanApp.Components.Common.ScanAppTable.Utilities
{
    /// <summary>
    /// Static class for handling nested properties, includes methods for
    /// getting and setting values.
    /// </summary>
    public static class NestedPropertyHandler
    {
        /// <summary>
        /// Gets a value of a specific nested <paramref name="propName"/> from an object <paramref name="src"/>.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static object GetNestedPropertyValue(object src, string propName)
        {
            if (src == null) throw new ArgumentException("Source object cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Name of the property to get cannot be null.", "propName");

            if (propName.Contains(".")) //complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetNestedPropertyValue(GetNestedPropertyValue(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.GetValue(src, null) : null;
            }
        }

        /// <summary>
        /// Sets <paramref name="value"/> of a specific nested <paramref name="propName"/> in a
        /// <paramref name="targetObj"/>.
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        public static void SetNestedPropertyValue(object targetObj, string propName, object value)
        {
            if (targetObj is null) throw new ArgumentNullException("targetObj", "Target object cannot be null.");
            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException("propName", "Property name cannot be null or empty.");

            var properties = propName.Split('.');

            for (int i = 0; i < (properties.Length - 1); i++)
            {
                var propertyToGet = targetObj.GetType().GetProperty(properties[i]);
                var propertyValue = propertyToGet?.GetValue(targetObj, null);
                if (propertyValue is null)
                {
                    if (propertyToGet.PropertyType.IsClass)
                    {
                        propertyValue = Activator.CreateInstance(propertyToGet.PropertyType);
                        propertyToGet.SetValue(targetObj, propertyValue);
                    }
                }
                targetObj = propertyValue;
            }
            var propertyToSet = targetObj.GetType().GetProperty(properties.Last());
            propertyToSet.SetValue(targetObj, value);
        }
    }
}