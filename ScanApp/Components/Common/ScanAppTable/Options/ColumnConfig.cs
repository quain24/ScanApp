using FluentValidation;
using FluentValidation.Results;
using ScanApp.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public class ColumnConfig<T>
    {
        private Expression<Func<T, object>> ColumnNameSelector { get; }

        private IValidator Validator { get; }
        public string PropertyName { get; }
        public IReadOnlyList<MemberInfo> PropertyPath { get; private set; }
        public string DisplayName { get; }
        public Type PropertyType { get; }
        public bool IsFilterable { get; init; } = true;
        public bool IsEditable { get; init; } = true;
        public bool IsGroupable { get; init; } = true;
        public Guid Identifier { get; } = Guid.NewGuid();

        public ColumnConfig(Expression<Func<T, object>> columnNameSelector, string displayName, IValidator validator) : this(columnNameSelector, displayName)
        {
            Validator = validator;
        }

        public ColumnConfig(Expression<Func<T, object>> columnNameSelector, string displayName)
        {
            ColumnNameSelector = columnNameSelector ?? throw new ArgumentNullException(nameof(columnNameSelector));
            PropertyPath = PropertyPath<T>.GetFrom(ColumnNameSelector);

            PropertyName = ExtractPropertyName();
            DisplayName = SetDisplayName(displayName);

            PropertyType = ExtractValidatedType();
        }

        private string ExtractPropertyName()
        {
            return PropertyPath.Count == 0
                ? typeof(T)?.Name
                : PropertyPath[^1]?.Name ?? throw new ArgumentException("Could not extract property name!");
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
            if (PropertyPath.Count == 0)
                return typeof(T);

            return PropertyPath[^1].MemberType switch
            {
                MemberTypes.Property => (PropertyPath[^1] as PropertyInfo)?.PropertyType,
                MemberTypes.Field => (PropertyPath[^1] as FieldInfo)?.FieldType,
                _ => throw new ArgumentException("Could not extract property type!")
            };
        }

        private Func<dynamic, IEnumerable<string>> CreateStrongTypeValidatorFrom(IValidator validator)
        {
            _ = validator ?? throw new ArgumentNullException(nameof(validator));
            return value =>
            {
                Type contextType = typeof(ValidationContext<>).MakeGenericType(PropertyType);
                ValidationResult result = validator.Validate(Activator.CreateInstance(contextType, value));
                return result.IsValid
                    ? Array.Empty<string>()
                    : ExtractErrorsFrom(result);
            };
        }

        public IEnumerable<string> Validate<TValueType>(TValueType value)
        {
            var context = new ValidationContext<TValueType>(value);
            var result = Validator.Validate(context);
            return result.IsValid
                ? Array.Empty<string>()
                : ExtractErrorsFrom(result);
        }

        private static IEnumerable<string> ExtractErrorsFrom(ValidationResult result)
        {
            var errors = new List<string>(result.Errors.Count);
            foreach (var failure in result.Errors)
            {
                errors.Add(failure.ErrorMessage);
            }
            return errors;
        }
    }
}