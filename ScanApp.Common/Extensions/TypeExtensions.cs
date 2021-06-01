﻿using System;

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

        /// <summary>
        /// Checks if give <paramref name="type"/> is considered to be a numeric one.
        /// <para>
        /// Numeric types considered here are all primitive data types excluding <see cref="char"/> and including <see cref="decimal"/>.<br/>
        /// In case of <see cref="Nullable{T}"/> values - underlying type will be checked.
        /// </para>
        /// </summary>
        /// <param name="type">Type to be checked</param>
        /// <returns><see langword="true"/> if type or, if nullable, underlying type is numeric; Otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Checked <paramref name="type"/> was <see langword="null"/>.</exception>
        public static bool IsNumeric(this Type type)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));
            var t = Nullable.GetUnderlyingType(type) ?? type;
            return (t.IsPrimitive && t != typeof(char)) || t == typeof(decimal);
        }
    }
}