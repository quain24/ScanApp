using System;
using System.Linq;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog
{
    public static class TypeChecker
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

        public static bool IsInteger(Type type)
        {
            return IntTypes.Contains(type);
        }

        public static bool IsInteger(object obj)
        {
            var type = obj.GetType();
            return IntTypes.Contains(type);
        }

        public static bool IsDecimal(Type type)
        {
            return DecimalTypes.Contains(type);
        }

        public static bool IsDecimal(object obj)
        {
            var type = obj.GetType();
            return DecimalTypes.Contains(type);
        }

        public static bool IsDateTime(Type type)
        {
            return DateTimeTypes.Contains(type);
        }

        public static bool IsDateTime(object obj)
        {
            var type = obj.GetType();
            return DateTimeTypes.Contains(type);
        }

        public static bool IsString(Type type)
        {
            return type == typeof(string);
        }

        public static bool IsString(object obj)
        {
            var type = obj.GetType();
            return type == typeof(string);
        }
    }
}