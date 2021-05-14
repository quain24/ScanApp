using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public class ColumnConfig<T>
    {
        private readonly string _errorMessage;
        private readonly Expression<Func<T, object>> _columnNameSelector;

        public Func<dynamic, IEnumerable<string>> Validator { get; }
        public string PropertyName { get; }
        public string DisplayName { get; }
        public Type PropertyType { get; }
        public bool IsFilterable { get; init; } = true;
        public bool IsEditable { get; init; } = true;
        public bool IsSelectable { get; init; } = true;
        public bool IsGroupable { get; init; } = true;

        public ColumnConfig(Expression<Func<T, object>> columnNameSelector, string displayName, IPropertyValidator validator, string errorMessage) : this(columnNameSelector, displayName)
        {
            _errorMessage = errorMessage;
            Validator = CreateStrongTypeValidatorFrom(validator);
        }

        public ColumnConfig(Expression<Func<T, object>> columnNameSelector, string displayName, IValidator validator) : this(columnNameSelector, displayName)
        {
            Validator = validator is not null ? CreateStrongTypeValidatorFrom(validator) : null;
        }

        public ColumnConfig(Expression<Func<T, object>> columnNameSelector, string displayName)
        {
            _columnNameSelector = columnNameSelector ?? throw new ArgumentNullException(nameof(columnNameSelector));

            PropertyName = ExtractPropertyName();
            DisplayName = SetDisplayName(displayName);

            PropertyType = ExtractValidatedType();
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

        private Func<dynamic, IEnumerable<string>> CreateStrongTypeValidatorFrom(IValidator validator)
        {
            var emptyContextType = typeof(ValidationContext<>);
            var contextType = emptyContextType.MakeGenericType(PropertyType);

            return value =>
            {
                var context = Activator.CreateInstance(contextType, value);
                var res = validator.Validate(context);

                if (res.IsValid)
                    return Array.Empty<string>();

                var errors = new List<string>(res.Errors.Count);
                foreach (var failure in res.Errors)
                {
                    errors.Add(failure.ErrorMessage);
                }

                return errors;
            };
        }

        private Func<dynamic, IEnumerable<string>> CreateStrongTypeValidatorFrom(IPropertyValidator validator)
        {
            var emptyContextType = typeof(PropertyValidator<,>);
            var contextType = emptyContextType.MakeGenericType(PropertyType, PropertyType);

            return value =>
            {
                dynamic vali = Convert.ChangeType(validator, validator.GetType());
                var res = vali.IsValid(null, value);

                if (res is true)
                    return Array.Empty<string>();
                return new List<string>(1)
                {
                    _errorMessage
                };
            };
        }
    }
}