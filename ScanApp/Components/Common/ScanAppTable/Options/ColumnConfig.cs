using FluentValidation;
using FluentValidation.Results;
using ScanApp.Components.Common.ScanAppTable.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public class ColumnConfig<T>
    {
        private readonly Expression<Func<T, object>> _columnNameSelector;
        public IValidator Validator { get; }
        public string PropertyName { get; }
        public string PropertyFullName { get;}
        public string DisplayName { get; }
        public Type PropertyType { get; }
        public PropertyInfo PropInfo { get; set; }
        public bool IsNested { get; private set; }
        public bool IsFilterable { get; init; } = true;
        public bool IsEditable { get; init; } = true;
        public bool IsSelectable { get; init; } = true;
        public bool IsGroupable { get; init; } = true;
        private object _default;
        private DateTimeFormat.Show _dateTimeFormat;

        public DateTimeFormat.Show DateTimeFormat
        {
            get => _dateTimeFormat;
            set
            {
                if (PropertyType.IsDateTime())
                    _dateTimeFormat = value;
                else
                    throw new ArgumentException("Cannot set DateTimeFormat for a property of type " + PropertyType.ToString(),
                        nameof(ColumnConfig<T>.DateTimeFormat));
            }
        }

        public object Default
        {
            get => _default;
            set
            {
                if (value.GetType() == PropertyType)
                    _default = value;
                else
                    throw new ArgumentException("Default value " + value.ToString() + " of type  " + value.GetType().ToString() + " provided is not of the same type as the property type " +
                                                PropertyType.ToString(),
                        nameof(ColumnConfig<T>.Default));
            }
        }

        public ColumnConfig(Expression<Func<T, object>> columnNameSelector, string displayName, IValidator validator = null)
        {
            _columnNameSelector = columnNameSelector ?? throw new ArgumentNullException(nameof(columnNameSelector));
            PropertyFullName = ExtractFullPropertyName();
            PropertyName = ExtractPropertyName();
            DisplayName = SetDisplayName(displayName);
            PropertyType = ExtractValidatedType();
            Validator = validator ?? null;
            PropInfo = ExtractPropertyInfo();
        }

        private PropertyInfo ExtractPropertyInfo()
        {
            try
            { 
                return _columnNameSelector.GetPropertyAccess();
            }
            catch
            {
                IsNested = true;
                return null;
            }
        }

        private string ExtractPropertyName()
        {
            return _columnNameSelector.Body switch
            {
                UnaryExpression { Operand: MemberExpression m } => m.Member.Name,
                MemberExpression m => m.Member.Name,
                { } m => m.Type.Name
            };
        }

        private string ExtractFullPropertyName()
        {
            var str = _columnNameSelector.Body.Print();
            return str.Substring(str.IndexOf('.')+1);
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