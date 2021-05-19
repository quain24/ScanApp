using FluentValidation;
using FluentValidation.Validators;
using ScanApp.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation.Results;
using ScanApp.Services;

namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public class ColumnConfigAlt<TContext, TProperty>
    {
        private readonly string _errorMessage;

        public Expression<Func<TContext, TProperty>> ColumnNameSelector { get; }

        public Func<TProperty, IEnumerable<string>> Validator { get; }
        public IValidator<TProperty> val { get; }
        public AbstractValidator<TContext> AbsVal { get; }
        public string PropertyName { get; }
        public IReadOnlyList<MemberInfo> PropertyPath { get; private set; }
        public string DisplayName { get; }
        public Type PropertyType { get; }
        public bool IsFilterable { get; init; } = true;
        public bool IsEditable { get; init; } = true;
        public bool IsSelectable { get; init; } = true;
        public bool IsGroupable { get; init; } = true;
        public Guid Identifier { get; } = Guid.NewGuid();

        public ColumnConfigAlt(Expression<Func<TContext, TProperty>> columnNameSelector, string displayName, IPropertyValidator<TProperty, TProperty> validator, string errorMessage) : this(columnNameSelector, displayName)
        {
            _errorMessage = errorMessage;
            Validator = CreateStrongTypeValidatorFrom(validator);
        }

        public ColumnConfigAlt(Expression<Func<TContext, TProperty>> columnNameSelector, string displayName, IValidator<TProperty> validator) : this(columnNameSelector, displayName)
        {
            Validator = validator is not null ? CreateStrongTypeValidatorFrom(validator) : null;
            val = validator;
        }

        public ColumnConfigAlt(Expression<Func<TContext, TProperty>> columnNameSelector, string displayName, AbstractValidator<TContext> validator) : this(columnNameSelector, displayName)
        {
            AbsVal = validator;
        }

        public ColumnConfigAlt(Expression<Func<TContext, TProperty>> columnNameSelector, string displayName)
        {
            ColumnNameSelector = columnNameSelector ?? throw new ArgumentNullException(nameof(columnNameSelector));
            PropertyPath = PropertyPath<TContext>.GetFrom(ColumnNameSelector);

            PropertyName = ExtractPropertyName();
            DisplayName = SetDisplayName(displayName);

            PropertyType = ExtractValidatedType();
        }

        private string ExtractPropertyName()
        {
            return PropertyPath.Count == 0
                ? typeof(TContext)?.Name
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
            return typeof(TProperty);
        }

        private Func<TProperty, IEnumerable<string>> CreateStrongTypeValidatorFrom(IValidator<TProperty> validator)
        {
            _ = validator ?? throw new ArgumentNullException(nameof(validator));
            
            return  value =>
            {
                ValidationResult result = validator.Validate(value);
                return result.IsValid
                    ? Array.Empty<string>()
                    : ExtractErrorsFrom(result);
            };
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

        private Func<TProperty, IEnumerable<string>> CreateStrongTypeValidatorFrom(IPropertyValidator<TProperty, TProperty> validator)
        {
            return value =>
            {
                var res = validator.IsValid(new ValidationContext<TProperty>(value), value);

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