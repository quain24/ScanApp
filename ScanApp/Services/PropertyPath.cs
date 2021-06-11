using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ScanApp.Services
{
    public static class PropertyPath<TSource>
    {
        public static IReadOnlyList<MemberInfo> GetFrom<TResult>(Expression<Func<TSource, TResult>> expression)
        {
            _ = expression ?? throw new ArgumentNullException(nameof(expression));
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

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                Path.Add(node.Method);
                return base.VisitMethodCall(node);
            }
        }
    }
}