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
                if (node.Member is not PropertyInfo)
                {
                    throw new ArgumentException("The path can only contain properties", nameof(node));
                }

                this.Path.Add(node.Member);
                return base.VisitMember(node);
            }
        }

        /// <summary>
        /// Provides simple way to modify value of given column row - to be used with <see cref="ScanAppTable{TItem}"/>
        /// </summary>
        /// <param name="columnConfig">Source column in which value should be changed.</param>
        /// <param name="source">Model representing single row of data displayed in table, which will have its data modified.</param>
        /// <param name="value">New data for given <paramref name="source"/>"/></param>
        /// <exception cref="ArgumentException">Given <paramref name="value"/> is of different type that one selected in <paramref name="columnConfig"/>.</exception>
        public static void SetValue(ColumnConfig<TSource> columnConfig, object source, object value)
        {
            if (columnConfig.PropertyType != value.GetType())
                throw new ArgumentException($"Given value type ({value.GetType()}) is different than property / field type being set ({columnConfig.PropertyType}).", nameof(value));
            _ = SetValuePrivate(columnConfig.PropertyPath.ToList(), source, value);
        }

        public static void SetValue(IEnumerable<MemberInfo> path, TSource source, object value)
        {
            if (path.Last() != value.GetType())
                throw new ArgumentException("Given value is of a different type than property / field being set.", nameof(value));
            _ = SetValuePrivate(path.ToList(), source, value);
        }

        private static object SetValuePrivate(List<MemberInfo> infos, object source, object value)
        {
            var currentInfo = infos[0];

            if (infos.Count == 1)
            {
                var current = Array.Find(source.GetType().GetMember(currentInfo.Name), m => m.MemberType == currentInfo.MemberType);

                switch (current?.MemberType)
                {
                    case MemberTypes.Field:
                        ((FieldInfo)currentInfo).SetValue(source, value);
                        break;

                    case MemberTypes.Property:
                        ((PropertyInfo)currentInfo).SetValue(source, value, null);
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
                _ => throw new ArgumentException("Only properties and fields are supported as writable.")
            };
            var newInfos = infos.GetRange(1, infos.Count - 1);

            return SetValuePrivate(newInfos, currentValue, value);
        }

        public static dynamic GetValue(ColumnConfig<TSource> columnConfig, TSource source)
        {
            return GetValuePrv(columnConfig.PropertyPath.ToList(), source);
        }

        private static dynamic GetValuePrv(List<MemberInfo> infos, object source)
        {
            var currentInfo = infos[0];

            if (infos.Count == 1)
            {
                var current = Array.Find(source.GetType().GetMember(currentInfo.Name), m => m.MemberType == currentInfo.MemberType);

                return current?.MemberType switch
                {
                    MemberTypes.Field => ((FieldInfo)currentInfo).GetValue(source),
                    MemberTypes.Property => ((PropertyInfo)currentInfo).GetValue(source),
                    _ => throw new ArgumentException("Only properties or fields are readable.")
                };
            }

            var currentValue = currentInfo.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)currentInfo).GetValue(source),
                MemberTypes.Property => ((PropertyInfo)currentInfo).GetValue(source),
                _ => throw new ArgumentException("Only properties and fields are supported as writable.")
            };
            var newInfos = infos.GetRange(1, infos.Count - 1);

            return GetValuePrv(newInfos, currentValue);
        }
    }
}