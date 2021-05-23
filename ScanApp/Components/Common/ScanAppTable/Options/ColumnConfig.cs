using FluentValidation;
using FluentValidation.Results;
using ScanApp.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public class ColumnConfig<T>
    {
        public string DisplayName { get; }
        public string PropertyName { get; }
        public Type PropertyType { get; }
        private Expression<Func<T, object>> ColumnNameSelector { get; }
        public Guid Identifier { get; } = Guid.NewGuid();
        public IReadOnlyList<MemberInfo> PropertyPath { get; }
        private IValidator Validator { get; }
        public bool IsFilterable { get; init; } = true;
        public bool IsEditable { get; init; } = true;
        public bool IsGroupable { get; init; } = true;
        public bool IsValidatable => Validator is not null;

        public ColumnConfig(Expression<Func<T, object>> columnNameSelector, string displayName, IValidator validator) : this(columnNameSelector, displayName)
        {
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public ColumnConfig(Expression<Func<T, object>> columnNameSelector, string displayName)
        {
            ColumnNameSelector = columnNameSelector ?? throw new ArgumentNullException(nameof(columnNameSelector));
            PropertyPath = PropertyPath<T>.GetFrom(ColumnNameSelector);

            PropertyName = ExtractPropertyName();
            DisplayName = SetDisplayName(displayName);

            PropertyType = ExtractPropertyType();
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

        private Type ExtractPropertyType()
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

        public IEnumerable<string> Validate<TValueType>(TValueType value)
        {
            if (Validator is null)
            {
                throw new ArgumentException("Cannot validate when there is no validator set - " +
                                            "perhaps editing field tried to use this config as one with validation?");
            }

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