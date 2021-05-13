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
        public bool IsFilterable { get; private init; }
        public bool IsEditable { get; private init; }
        public bool IsSelectable { get; private init; }
        public bool IsGroupable { get; private init; }
        private IValidator cc;

        public ColumnConfig(Expression<Func<T, object>> columnNameSelector, string displayName, bool isFilterable = true,
            bool isEditable = true, bool isSelectable = true, bool isGroupable = true)
        {
            _columnNameSelector = columnNameSelector ?? throw new ArgumentNullException(nameof(columnNameSelector));

            PropertyName = ExtractPropertyName();
            DisplayName = displayName switch
            {
                null => PropertyName,
                var s when string.IsNullOrWhiteSpace(s) => throw new ArgumentException("Display name cannot contain only whitespaces.", nameof(displayName)),
                _ => displayName
            };

            IsFilterable = isFilterable;
            IsEditable = isEditable;
            IsSelectable = isSelectable;
            IsGroupable = isGroupable;
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