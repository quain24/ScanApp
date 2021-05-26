using System;

namespace ScanApp.Common.Extensions
{
    public static class TypeExtensions
    {
        public static dynamic GetDefaultValue(this Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            return null;
        }

        public static bool IsNumeric(this Type value)
        {
            var t = Nullable.GetUnderlyingType(value) ?? value;
            return (t.IsPrimitive && t != typeof(char)) || t == typeof(decimal);
        }
    }
}