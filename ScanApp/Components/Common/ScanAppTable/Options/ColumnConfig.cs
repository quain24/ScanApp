using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public class ColumnConfig<T>
    {
        private readonly Expression<Func<T, object>> _columnNameSelector;
        public IValidator Validator { get; }
        public string PropertyName { get; }
        public string DisplayName { get; }
        public Type PropertyType { get; }
        public bool IsFilterable { get; init; } = true;
        public bool IsEditable { get; init; } = true;
        public bool IsSelectable { get; init; } = true;
        public bool IsGroupable { get; init; } = true;

        public ColumnConfig(Expression<Func<T, object>> columnNameSelector, string displayName, IValidator validator = null)
        {
            _columnNameSelector = columnNameSelector ?? throw new ArgumentNullException(nameof(columnNameSelector));
            PropertyName = ExtractPropertyName();
            DisplayName = SetDisplayName(displayName);
            PropertyType = ExtractValidatedType();
            Validator = validator ?? null;
        }

        private string ExtractPropertyName()
        {
            return _columnNameSelector.Body switch
            {
                UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary => unary.Operand.Type.Name,
                UnaryExpression { Operand: MemberExpression m } => m.Member.Name,
                MemberExpression m => m.Member.Name,
                { } m => m.Type.Name
            };
        }

        private string SetDisplayName(string name)
        {
            return name switch
            {
                null => PropertyName,
                var s when string.IsNullOrWhiteSpace(s) => throw new ArgumentException("Display name cannot contain only whitespaces.", nameof(name)),
                _ => name
            };
        }

        private Type ExtractValidatedType()
        {
            return _columnNameSelector.Body switch
            {
                { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } and UnaryExpression unary => unary.Operand.Type,
                { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } and MemberExpression member => member.Type,
                _ => _columnNameSelector.Body.Type
            };
        }

        private IEnumerable<string> ExtractErrorsFrom(ValidationResult validationResult)
        {
            var errors = new List<string>(validationResult.Errors.Count);
            foreach (var failure in validationResult.Errors)
            {
                errors.Add(failure.ErrorMessage);
            }
            return errors;
        }


        public Func<TValue, IEnumerable<string>> ToMudFormFieldValidator<TValue>()
        {
            if (Validator is null)
                return null;
            
            _ = Validator;
            return value =>
            {
                if (value is null)
                {
                    return Array.Empty<string>();
                }
                var context = new ValidationContext<TValue>(value);
                var res = Validator.Validate(context);
                if (res.IsValid)
                    return Array.Empty<string>();

                return ExtractErrorsFrom(res);
            };
        }

        public IEnumerable<string> Validate<TValue>(TValue value)
        {
            if (Validator is null)
            {
                return Array.Empty<string>();
            }
            var validationContext = new ValidationContext<TValue>(value);
            var validationResult = Validator.Validate(validationContext);
            return validationResult.IsValid
                ? Array.Empty<string>()
                : ExtractErrorsFrom(validationResult);
        }

    }
}