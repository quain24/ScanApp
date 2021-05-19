using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ScanApp.Common.Services
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
    }
}