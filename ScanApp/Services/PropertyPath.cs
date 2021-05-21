using ScanApp.Components.Common.AltTableTest;
using ScanApp.Components.Common.ScanAppTable.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ScanApp.Services
{
    public static class PropertyPath<TSource>
    {
        public static IReadOnlyList<MemberInfo> GetFrom<TResult>(Expression<Func<TSource, TResult>> expression)
        {
            var visitor = new PropertyVisitor();
            visitor.Visit(expression.Body);
            visitor.Path.Reverse();
            return visitor.Path;
        }

        private class PropertyVisitor : ExpressionVisitor
        {
            internal readonly List<MemberInfo> Path = new();

            protected override Expression VisitMember(MemberExpression node)
            {
                Path.Add(node.Member);
                return base.VisitMember(node);
            }
        }

        /// <summary>
        /// Provides simple way to modify value of given column's row - to be used with <see cref="Alttablecomponent{TTableType}"/>
        /// </summary>
        /// <param name="columnConfig">Source column in which value should be changed.</param>
        /// <param name="source">Model representing single row of data displayed in table, which will have its data modified.</param>
        /// <param name="value">New data for given <paramref name="source"/></param>
        /// <exception cref="ArgumentException">Given <paramref name="value"/> is of different type that one selected in <paramref name="columnConfig"/>.</exception>
        public static void SetValue(ColumnConfig<TSource> columnConfig, TSource source, dynamic value)
        {
            if (CheckValueCompatibility(columnConfig.PropertyType, value) is false)
            {
                throw new ArgumentException($"Given {nameof(value)}'s type ({value?.GetType().Name ?? $"{nameof(value)} was NULL"}) is different than property" +
                                            $" / field type being set ({columnConfig.PropertyType}) using {nameof(columnConfig)} for variable named {columnConfig.DisplayName}" +
                                            $" (Identifier - {columnConfig.Identifier}).", nameof(value));
            }

            _ = SetValueRecursive(columnConfig.PropertyPath as List<MemberInfo>, source, value);
        }

        private static bool CheckValueCompatibility(Type storedType, dynamic value)
        {
            return storedType switch
            {
                var pt when Nullable.GetUnderlyingType(pt) is null && value is null => false,
                var pt when Nullable.GetUnderlyingType(pt) is null && Nullable.GetUnderlyingType(value.GetType()) is null => pt == value.GetType(),
                var pt when Nullable.GetUnderlyingType(pt) is not null && value is null => true,
                var pt when Nullable.GetUnderlyingType(pt) is not null && Nullable.GetUnderlyingType(value.GetType()) is not null => pt == value.GetType(),
                var pt when Nullable.GetUnderlyingType(pt) is not null => value.GetType() == Nullable.GetUnderlyingType(pt),
                var pt when pt.IsValueType && value is null => false,
                _ => false
            };
        }

        public static void SetValue(IList<MemberInfo> path, TSource source, dynamic value)
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
                MemberInfo memberToBeSet = null;
                MemberInfo[] members = source.GetType().GetMember(currentInfo.Name, BindingFlags.ExactBinding | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var member in members)
                {
                    if (member.MemberType == currentInfo.MemberType)
                        memberToBeSet = member;
                }

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

            var currentValue = currentInfo.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)currentInfo).GetValue(source),
                MemberTypes.Property => ((PropertyInfo)currentInfo).GetValue(source),
                _ => throw new ArgumentException("Can only iterate through properties or variables while trying to set values.")
            };
            var newInfos = infos.GetRange(1, infos.Count - 1);

            return SetValueRecursive(newInfos, currentValue, value);
        }

        public static dynamic GetValue(ColumnConfig<TSource> columnConfig, TSource source)
        {
            return GetValueRecursive(columnConfig.PropertyPath as List<MemberInfo>, source);
        }

        private static dynamic GetValueRecursive(List<MemberInfo> infos, dynamic source)
        {
            if (source is null)
                return null;

            var currentInfo = infos[0];

            if (infos.Count == 1)
            {
                MemberInfo memberToBeRead = null;
                MemberInfo[] members = source.GetType().GetMember(currentInfo.Name, BindingFlags.ExactBinding | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var member in members)
                {
                    if (member.MemberType == currentInfo.MemberType)
                        memberToBeRead = member;
                }

                return memberToBeRead?.MemberType switch
                {
                    MemberTypes.Field => ((FieldInfo)memberToBeRead).GetValue(source),
                    MemberTypes.Property => ((PropertyInfo)memberToBeRead).GetValue(source),
                    _ => throw new ArgumentException("Only properties or fields are readable.")
                };
            }

            var currentValue = currentInfo.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)currentInfo).GetValue(source),
                MemberTypes.Property => ((PropertyInfo)currentInfo).GetValue(source),
                _ => throw new ArgumentException("Can only iterate through properties or variables while trying to read values.")
            };
            var newInfos = infos.GetRange(1, infos.Count - 1);

            return GetValueRecursive(newInfos, currentValue);
        }
    }
}