using System;
using System.Linq;

namespace ScanApp.Components.Common.ScanAppTable.Extensions
{
    public static class TypeExtensions
    {
        private static readonly Type[] IntTypes = new Type[]
        {
            typeof(sbyte), typeof(sbyte?),
            typeof(byte), typeof(byte?),
            typeof(short), typeof(short?),
            typeof(ushort), typeof(ushort?),
            typeof(int), typeof(int?),
            typeof(uint), typeof(uint?),
            typeof(long), typeof(long?),
            typeof(ulong), typeof(ulong?),
            typeof(nint), typeof(nint?),
            typeof(nuint), typeof(nuint?)
        };

        private static readonly Type[] DecimalTypes = new Type[]
        {
            typeof(decimal), typeof(decimal?),
            typeof(double), typeof(double?),
            typeof(float), typeof(float?)
        };

        private static readonly Type[] DateTimeTypes = new Type[]
        {
            typeof(DateTime), typeof(DateTime?),
            typeof(DateTimeOffset), typeof(DateTimeOffset?)
        };

        /// <summary>
        /// Checks if <paramref name="type"/> (including nullable types) is an integral type.
        /// </summary>
        /// <param name="type"></param>
        public static bool IsInteger(this Type type)
        {
            return IntTypes.Contains(type);
        }

        /// <summary>
        /// Checks if <paramref name="type"/> (including nullable types) is a floating-point numeric type.
        /// </summary>
        /// <param name="type"></param>
        public static bool IsDecimal(this Type type)
        {
            return DecimalTypes.Contains(type);
        }

        /// <summary>
        /// Checks if <paramref name="type"/> (including nullable types) is a DateTime type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDateTime(this Type type)
        {
            return DateTimeTypes.Contains(type);
        }

        /// <summary>
        /// Checks if <paramref name="type"/> is a string type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsString(this Type type)
        {
            return type == typeof(string);
        }

        /// <summary>
        /// Checks if <paramref name="type"/> is either an integral type or a floating-point numeric type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIntegerOrDecimal(this Type type)
        {
            return IsInteger(type) || IsDecimal(type);
        }

        /// <summary>
        /// Gets a nullable version of a <paramref name="type"/> provided.
        /// If a nullable version does not exist or the <paramref name="type"/> passed
        /// is a nullable - an unchanged <paramref name="type"/> type is returned.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNullableType(this Type type)
        {
            try
            {
                type = Nullable.GetUnderlyingType(type) ?? type;
            }
            catch
            {
                return null;
            }
            if (type.IsValueType)
                return typeof(Nullable<>).MakeGenericType(type);
            else
                return type;
        }
    }
}