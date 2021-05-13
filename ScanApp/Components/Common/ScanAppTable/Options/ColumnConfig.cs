using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.Logging;
using ScanApp.Common;
using ScanApp.Models.SpareParts;

namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public class ColumnConfig<T>
    {
        public Expression<Func<T, object>> _columnNameSelector { get; init; }
        public string PropertyName { get; }
        public string DisplayName { get;}
        public bool IsFilterable { get; init; } = true;
        public bool IsEditable { get; init; } = true;
        public bool IsSelectable { get; init; } = true;
        public bool IsGroupable { get; init; } = true;

        public ColumnConfig(Expression<Func<T, object>> columnNameSelector, string displayName)
        {
            _columnNameSelector = columnNameSelector ?? throw new ArgumentNullException(nameof(columnNameSelector));

            PropertyName = ExtractPropertyName();
            DisplayName = displayName switch
            {
                null => PropertyName,
                var s when string.IsNullOrWhiteSpace(s) => throw new ArgumentException("Display name cannot contain only whitespaces.", nameof(displayName)),
                _ => displayName
            };
        }

        private string ExtractPropertyName()
        {
            return _columnNameSelector.Body switch
            {
                MemberExpression m => m.Member.Name,
                UnaryExpression { Operand: MemberExpression m } => m.Member.Name,
                { } m => m.Type.Name
            };
        }

    }
}