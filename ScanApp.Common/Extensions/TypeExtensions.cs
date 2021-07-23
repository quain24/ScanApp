using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static bool CheckValueCompatibility(this Type type, object value)
        {
            if (value is null)
                return Nullable.GetUnderlyingType(type) is not null || type.GetDefaultValue() is null;

            return type switch
            {
                var pt when pt == value.GetType() => true,
                var pt when pt.IsInstanceOfType(value) => true,
                var pt when Nullable.GetUnderlyingType(pt) == value.GetType() => true,
                var pt when Nullable.GetUnderlyingType(pt) is not null && Nullable.GetUnderlyingType(pt) == Nullable.GetUnderlyingType(value.GetType()) => true,
                _ => false
            };
        }

        public static IEnumerable<Type> GetImplementingTypes(this Type type, Assembly assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            if (!type.IsGenericTypeDefinition)
                return assembly.GetTypes().Where(x => x.IsAssignableFrom(type) && x.IsClass && !x.IsAbstract).ToList();

            return assembly
                .GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type));
        }

        public static IEnumerable<Type> GetAllTypesFromGeneric(this Type genericType, Assembly assembly = null, params Type[] genericParameterTypes)
        {
            if (!genericType.IsGenericTypeDefinition)
                throw new ArgumentException("Specified type must be a generic type definition.", nameof(genericType));

            return (assembly ?? Assembly.GetExecutingAssembly())
                .GetTypes()
                .Where(t =>
                    t.GetInterfaces()
                    .Any(type => type.IsGenericType &&
                         type.GetGenericTypeDefinition() == genericType &&
                         type.GetGenericArguments().Length == genericParameterTypes.Length &&
                         type.GetGenericArguments()
                            .Zip(genericParameterTypes, (f, s) => s.IsAssignableFrom(f))
                            .All(z => z)));
        }
    }
}