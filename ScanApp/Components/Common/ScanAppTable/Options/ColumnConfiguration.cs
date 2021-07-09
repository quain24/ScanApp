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
    public class ColumnConfiguration<T>
    {
        private readonly Expression<Func<T, object>> _columnNameSelector;
        /// <summary>
        /// Validator that will be plugged into editing and filtering fields in the table.
        /// </summary>
        public IValidator Validator { get; }

        /// <summary>
        /// Short property name. Ignores hierarchy of nested objects, if exists.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Full name of the property including hierarchy of objects if property is nested.
        /// Used in sorting, filtering, grouping for property identification.
        /// </summary>
        public string PropertyFullName { get;}

        /// <summary>
        /// String that will be displayed in the column header.
        /// </summary>
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
        /// <summary>
        /// Represents how <see cref="DateTime"/> properties will be displayed in the table.
        /// </summary>
        public DateTimeFormat.Show DateTimeFormat
        {
            get => _dateTimeFormat;
            set
            {
                if (PropertyType.IsDateTime())
                    _dateTimeFormat = value;
                else
                    throw new ArgumentException("Cannot set DateTimeFormat for a property of type " + PropertyType.ToString(),
                        nameof(ColumnConfiguration<T>.DateTimeFormat));
            }
        }
        /// <summary>
        /// Default value of the object that the user will be forced to use
        /// while adding a new item in the table.
        /// </summary>
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
                        nameof(ColumnConfiguration<T>.Default));
            }
        }

        public ColumnConfiguration(Expression<Func<T, object>> columnNameSelector, string displayName, IValidator validator = null)
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
        /// <summary>
        /// Transforms validator into a form that is accepted by <see cref="MudBlazor"/> text fields.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
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
        /// <summary>
        /// Validates a property. Returns <see cref="IEnumerable{T}"/> that represents
        /// any validation errors.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
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