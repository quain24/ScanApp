using FluentValidation;
using ScanApp.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public class ColumnConfig<T>
    {
        public string DisplayName { get; }
        public string PropertyName { get; }
        public Type PropertyType { get; }
        public FieldType FieldType { get; }
        public Func<dynamic, string> DisplayFormatter { get; }
        public Guid Identifier { get; } = Guid.NewGuid();
        public IReadOnlyList<MemberInfo> PropertyPath { get; }
        public bool IsFilterable { get; init; } = true;
        public bool IsEditable { get; init; } = true;
        public bool IsGroupable { get; init; } = true;
        public bool IsValidatable => Validator is not null;
        private IValidator Validator { get; }
        private Expression<Func<T, object>> ColumnNameSelector { get; }

        public ColumnConfig(Expression<Func<T, object>> target)
            : this(target, null, FieldType.AutoDetect, null, null)
        {
        }

        public ColumnConfig(Expression<Func<T, object>> target, string displayName)
            : this(target, displayName, FieldType.AutoDetect, null, null)
        {
        }

        public ColumnConfig(Expression<Func<T, object>> target, string displayName, IValidator validator)
            : this(target, displayName, FieldType.AutoDetect, null, validator)
        {
        }

        public ColumnConfig(Expression<Func<T, object>> target, string displayName, FieldType format)
            : this(target, displayName, format, null, null)
        {
        }

        public ColumnConfig(Expression<Func<T, object>> target, string displayName, FieldType format, IValidator validator)
            : this(target, displayName, format, null, validator)
        {
        }

        public ColumnConfig(Expression<Func<T, object>> target, string displayName, Func<dynamic, string> formatter)
            : this(target, displayName, FieldType.AutoDetect, formatter, null)
        {
        }

        public ColumnConfig(Expression<Func<T, object>> target, string displayName, Func<dynamic, string> formatter, IValidator validator)
            : this(target, displayName, FieldType.AutoDetect, formatter, validator)
        {
        }

        private ColumnConfig(Expression<Func<T, object>> target, string displayName, FieldType format, Func<dynamic, string> formatter, IValidator validator)
        {
            ColumnNameSelector = target ?? throw new ArgumentNullException(nameof(target));
            PropertyPath = PropertyPath<T>.GetFrom(ColumnNameSelector);

            PropertyName = ExtractPropertyName();
            DisplayName = SetDisplayName(displayName);

            PropertyType = ExtractPropertyType();
            FieldType = format;
            DisplayFormatter = formatter;
            Validator = validator;
            if (Validator?.CanValidateInstancesOfType(PropertyType) is false)
            {
                throw new ArgumentException($"Given validator cannot validate field/property of type '{PropertyType.Name}'" +
                                            $" pointed to by this {nameof(ColumnConfig<T>)} - GUID - {Identifier}.");
            }
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
            return PropertyPath.Count == 0 ? typeof(T) : PropertyPath[^1].GetUnderlyingType();
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

    public enum FieldType
    {
        AutoDetect = 0,
        Date,
        Time,
        DateAndTime,
        PlainText
    }
}