using SharedExtensions;
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

        /// <summary>
        /// Provides all implementation of given <paramref name="type"/> <b>interface</b>.
        /// </summary>
        /// <param name="type">Interface to be checked.</param>
        /// <param name="assemblies">List of assemblies to be scanned for implementations of <paramref name="type"/>.</param>
        /// <returns>List of implementations of given <paramref name="type"/>.</returns>
        public static IEnumerable<Type> GetImplementingTypes(this Type type, params Assembly[] assemblies)
        {
            if (type.IsInterface is false)
                throw new ArgumentException($"{nameof(type)} must be an interface. To get objects" +
                                            $" derived from {nameof(type)} use {nameof(Type)}.{nameof(GetDerivingTypes)}");

            assemblies = assemblies.IsNullOrEmpty()
                ? AppDomain.CurrentDomain.GetAssemblies()
                : assemblies;

            if (!type.IsGenericTypeDefinition)
                return assemblies.SelectMany(a => a.GetTypes().Where(x => x.IsAssignableFrom(type) && x.IsClass && !x.IsAbstract)).ToList();

            return assemblies.SelectMany(a => a
                .GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type)))
                .ToList();
        }

        /// <summary>
        /// Provides all types that derive from given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type to be checked.</param>
        /// <param name="assemblies">List of assemblies to be scanned for implementations of <paramref name="type"/>.</param>
        /// <returns>List of types derived from given <paramref name="type"/>.</returns>
        public static IEnumerable<Type> GetDerivingTypes(this Type type, params Assembly[] assemblies)
        {
            assemblies = assemblies.IsNullOrEmpty() ? AppDomain.CurrentDomain.GetAssemblies() : assemblies;

            if (!type.IsGenericTypeDefinition)
                return assemblies.SelectMany(a =>
                    a.GetTypes()
                        .Where(x => x.IsAssignableFrom(type) && !x.IsAbstract && x != type)).ToList();

            return assemblies.SelectMany(a => a
                .GetTypes()
                .Where(t => t.IsSubclassOfDeep(type)))
                .ToList();
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

        /// <inheritdoc cref="Type.IsSubclassOf"/>
        /// <remarks>
        /// This implementation checks for implementations of given <param name="type"> also in nested types.</param>
        /// </remarks>
        public static bool IsSubclassOfDeep(this Type type, Type baseType)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            if (baseType == null || type == baseType)
                return false;

            if (baseType.IsGenericType == false)
            {
                if (type.IsGenericType == false)
                    return type.IsSubclassOf(baseType);
            }
            else
            {
                baseType = baseType.GetGenericTypeDefinition();
            }

            type = type.BaseType;
            var objectType = typeof(object);
            while (type != objectType && type != null)
            {
                Type currentType = type.IsGenericType ?
                    type.GetGenericTypeDefinition() : type;
                if (currentType == baseType)
                    return true;

                type = type.BaseType;
            }

            return false;
        }
    }
}