using ScanApp.Components.Common.Table;
using ScanApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScanApp.Common.Extensions
{
    public static class ColumnConfigExtensions
    {
        /// <summary>
        /// Set <paramref name="value"/> in <paramref name="target"/> by using path from used <paramref name="columnConfig"/> (<see cref="ColumnConfig{TSource}"/>).
        /// </summary>
        /// <typeparam name="TSource">Type of source object.</typeparam>
        /// <param name="columnConfig">Source column containing information about which value / property should be set in <paramref name="target"/>.</param>
        /// <param name="target">Object that will have one of it's fields / properties set to given <paramref name="value"/>.</param>
        /// <param name="value">New data for given <paramref name="target"/></param>
        /// <exception cref="ArgumentException">Given <paramref name="value"/> is of different type that one selected in <paramref name="columnConfig"/>.</exception>
        public static void SetValue<TSource>(this ColumnConfig<TSource> columnConfig, TSource target, dynamic value)
        {
            if (columnConfig is null)
                throw new ArgumentNullException(nameof(columnConfig));
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            if (CheckValueCompatibility(columnConfig.PropertyType, value) is false)
            {
                throw new ArgumentException($"Given {nameof(value)}'s type ({value?.GetType().Name ?? $"{nameof(value)} was NULL"}) is different than property" +
                                            $" / field type being set ({columnConfig.PropertyType}) using {nameof(columnConfig)} for variable named '{columnConfig.DisplayName}'" +
                                            $" (Identifier - {columnConfig.Identifier}).", nameof(value));
            }

            _ = SetValueRecursive(columnConfig.PropertyPath as List<MemberInfo>, target, value);
        }

        private static bool CheckValueCompatibility(Type storedType, dynamic value)
        {
            if (value is null)
                return Nullable.GetUnderlyingType(storedType) is not null || storedType.GetDefaultValue() is null;

            return storedType switch
            {
                var pt when pt == value.GetType() => true,
                var pt when Nullable.GetUnderlyingType(pt) == value.GetType() => true,
                var pt when Nullable.GetUnderlyingType(pt) is not null && Nullable.GetUnderlyingType(pt) == Nullable.GetUnderlyingType(value.GetType()) => true,
                _ => false
            };
        }

        public static void SetValue<TSource>(this IList<MemberInfo> path, TSource source, dynamic value)
        {
            var checkedType = path.Last().GetUnderlyingType();
            if (CheckValueCompatibility(checkedType, value))
            {
                throw new ArgumentException($"Given {nameof(value)}'s type ({value?.GetType().Name ?? $"{nameof(value)} was NULL"})" +
                                            $" is different than property / field type being set ({checkedType}).");
            }

            _ = SetValueRecursive(path as List<MemberInfo>, source, value);
        }

        private static dynamic SetValueRecursive(List<MemberInfo> infos, dynamic source, dynamic value)
        {
            var currentInfo = infos[0];
            if (infos.Count == 1)
            {
                MemberInfo memberToBeSet = GetTargetedObjectMemberInfo(source, currentInfo);

                switch (memberToBeSet?.MemberType)
                {
                    case MemberTypes.Field:
                        ((FieldInfo)memberToBeSet).SetValue(source, value);
                        break;

                    case MemberTypes.Property:
                        ((PropertyInfo)memberToBeSet).SetValue(source, value, null);
                        break;

                    default:
                        throw new ArgumentException("Only properties or fields are writable.");
                }
                return source;
            }

            var currentSource = GetNewSourceFrom(currentInfo, source);
            var newInfos = infos.GetRange(1, infos.Count - 1);

            return SetValueRecursive(newInfos, currentSource, value);
        }

        private static MemberInfo GetTargetedObjectMemberInfo(dynamic source, MemberInfo currentInfo)
        {
            MemberInfo[] members = source
                .GetType()
                .GetMember(currentInfo.Name, BindingFlags.ExactBinding | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return Array.Find(members, member => member.MemberType == currentInfo.MemberType);
        }

        private static dynamic GetNewSourceFrom(MemberInfo currentInfo, dynamic source)
        {
            return currentInfo.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)currentInfo).GetValue(source),
                MemberTypes.Property => ((PropertyInfo)currentInfo).GetValue(source),
                _ => throw new ArgumentException("New source for recursive part of getting or setting value can only be property or field." +
                                                 "Possibly path to value contains something else?")
            };
        }

        /// <summary>
        /// Extracts underlying value (value source is pointed to in source <see cref="ColumnConfig{T}"/>) from given <paramref name="source"/>.<br/>
        /// Values can be extracted only from properties or fields stored in <paramref name="source"/>.
        /// Any depth is supported - given <paramref name="columnConfig"/> can point to some deeply nested property.<br/>
        /// <see langword="Static"/> fields and properties are not supported.
        /// </summary>
        /// <typeparam name="TSource">Type of source object from which value should be extracted.</typeparam>
        /// <param name="columnConfig">Configuration containing path to value being extracted.</param>
        /// <param name="source">Object from which we are trying to get a value.</param>
        /// <returns>Value extracted from <paramref name="source"/> using path stored in <paramref name="columnConfig"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="columnConfig"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value being extracted was something other than Property or Field.</exception>
        /// <exception cref="ArgumentException">Element on the path to value being extracted was something other than Property or Field.</exception>
        public static dynamic GetValueFrom<TSource>(this ColumnConfig<TSource> columnConfig, TSource source)
        {
            if (columnConfig is null)
                throw new ArgumentNullException(nameof(columnConfig));

            return source is null ? null : GetValueRecursive(columnConfig.PropertyPath as List<MemberInfo>, source);
        }

        private static dynamic GetValueRecursive(List<MemberInfo> infos, dynamic source)
        {
            if (source is null) return null;
            var currentInfo = infos[0];

            if (infos.Count == 1)
            {
                MemberInfo memberToBeRead = GetTargetedObjectMemberInfo(source, currentInfo);

                return memberToBeRead?.MemberType switch
                {
                    MemberTypes.Field => ((FieldInfo)memberToBeRead).GetValue(source),
                    MemberTypes.Property => ((PropertyInfo)memberToBeRead).GetValue(source),
                    _ => throw new ArgumentException("Only properties or fields are readable.")
                };
            }

            var currentSource = GetNewSourceFrom(currentInfo, source);
            var newInfos = infos.GetRange(1, infos.Count - 1);

            return GetValueRecursive(newInfos, currentSource);
        }
    }
}