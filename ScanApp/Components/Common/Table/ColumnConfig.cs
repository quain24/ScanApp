using FluentValidation;
using MudBlazor;
using ScanApp.Common.Extensions;
using ScanApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TypeExtensions = ScanApp.Common.Extensions.TypeExtensions;
using ValidationResult = FluentValidation.Results.ValidationResult;

// ReSharper disable IntroduceOptionalParameters.Global

namespace ScanApp.Components.Common.Table
{
    public class ColumnConfig<T>
    {
        public Guid Identifier { get; } = Guid.NewGuid();
        public string DisplayName { get; }
        public string PropertyName { get; }
        public Type PropertyType { get; }
        public FieldType FieldType { get; }
        public dynamic Converter { get; private set; }
        public bool IsFilterable { get; init; } = true;
        public bool IsEditable { get; init; } = true;
        public bool IsGroupable { get; init; } = true;
        private IValidator Validator { get; }
        private IReadOnlyList<MemberInfo> PathToItem { get; }
        private Expression<Func<T, dynamic>> TargetItemSelector { get; }
        private Func<T, dynamic> _getter;
        private Action<T, dynamic> _setter;
        private Func<T, dynamic, T> _valueSetter;

        public ColumnConfig(Expression<Func<T, dynamic>> target)
            : this(target, null, FieldType.AutoDetect, null)
        {
        }

        public ColumnConfig(Expression<Func<T, dynamic>> target, string displayName)
            : this(target, displayName, FieldType.AutoDetect, null)
        {
        }

        public ColumnConfig(Expression<Func<T, dynamic>> target, string displayName, IValidator validator)
            : this(target, displayName, FieldType.AutoDetect, validator)
        {
        }

        public ColumnConfig(Expression<Func<T, dynamic>> target, string displayName, FieldType format)
            : this(target, displayName, format, null)
        {
        }

        public ColumnConfig(Expression<Func<T, dynamic>> target, string displayName, FieldType format, IValidator validator)
        {
            TargetItemSelector = target ?? throw new ArgumentNullException(nameof(target));
            PathToItem = PropertyPath<T>.GetFrom(TargetItemSelector);

            PropertyName = ExtractPropertyName();
            PropertyType = ExtractPropertyType();
            DisplayName = SetDisplayName(displayName);

            CreatePrecompiledGetterForItem();
            CreatePrecompiledSetterForItem();
            ChooseSetValueVersion();
            FieldType = format;
            Validator = validator;
            if (Validator?.CanValidateInstancesOfType(PropertyType) is false)
            {
                throw new ArgumentException($"Given validator cannot validate field/property of type '{PropertyType.FullName}'" +
                                            $" pointed to by this {nameof(ColumnConfig<T>)} - GUID - {Identifier} | Property name - {PropertyName}.");
            }
        }

        public ColumnConfig<T> AssignConverter<TType>(Converter<TType> converter)
        {
            if (typeof(TType) != PropertyType)
            {
                throw new ArgumentException($"Given converter does not output compatible type (property - {PropertyType.FullName}), converter - {typeof(TType).FullName})");
            }
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
            return this;
        }

        private string ExtractPropertyName()
        {
            return PathToItem.Count == 0
                ? typeof(T)?.Name
                : PathToItem[^1]?.Name ?? throw new ArgumentException("Could not extract property name!");
        }

        private Type ExtractPropertyType() => PathToItem.Count == 0 ? typeof(T) : PathToItem[^1].GetUnderlyingType();

        private string SetDisplayName(string name)
        {
            return name switch
            {
                null => PropertyName,
                var s when string.IsNullOrWhiteSpace(s) => throw new ArgumentException("Display name cannot contain only whitespaces.", nameof(name)),
                _ => name
            };
        }

        private void CreatePrecompiledGetterForItem() => _getter = TargetItemSelector.Compile();

        private void CreatePrecompiledSetterForItem()
        {
            var valueParameterExpression = Expression.Parameter(typeof(object));
            var targetExpression = TargetItemSelector.Body is UnaryExpression unaryExpression ? unaryExpression.Operand : TargetItemSelector.Body;

            var assign = Expression.Lambda<Action<T, dynamic>>
            (
                Expression.Assign(targetExpression,
                    Expression.Convert(valueParameterExpression, targetExpression.Type)),
                TargetItemSelector.Parameters.Single(),
                valueParameterExpression
            );

            _setter = assign.Compile();
        }

        private void ChooseSetValueVersion()
        {
            _valueSetter = PathToItem switch
            {
                var p when p.Count == 0 => SetValueDirect,
                var p when p.Count == 1 && typeof(T).IsValueType => TriedSetImmutableValue,
                var p when p[^1].ReflectedType?.IsValueType ?? true => TriedSetImmutableValue,
                _ => SetValueWhenValid
            };
        }

        /// <summary>
        /// Set <paramref name="value"/> in <paramref name="target"/> if <paramref name="target"/> is a reference type.
        /// </summary>
        /// <param name="target">
        ///     If <paramref name="target"/> is a reference type, than it will have one of it's fields / properties set to given <paramref name="value"/> by this method.<br/>
        ///     <b>Otherwise <paramref name="target"/> will not be mutated.</b>
        /// </param>
        /// <param name="value">New data for given <paramref name="target"/></param>
        /// <returns>
        /// <para>
        ///     <paramref name="target"/> is a reference type - a reference to mutated <paramref name="target"/> will be returned.
        /// </para>
        /// <para>
        ///     <paramref name="target"/> is a value type - new value will be returned, <paramref name="target"/> will not be mutated,
        /// </para>
        /// </returns>
        /// <exception cref="ArgumentException">Given <paramref name="value"/> is of incompatible type to one stored in <see cref="PropertyType"/>.</exception>
        public T SetValue(T target, dynamic value) => _valueSetter(target, value);

        private static T SetValueDirect(T target, dynamic value) => value;

        private static T TriedSetImmutableValue(T target, dynamic value) => throw new ArgumentException("Cannot set values inside value types.");

        private T SetValueWhenValid(T target, dynamic value)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));

            if (!TypeExtensions.CheckValueCompatibility(PropertyType, value))
            {
                throw new ArgumentException($"Given {nameof(value)}'s type ({value?.GetType().Name ?? $"{nameof(value)} was NULL"}) is different than property" +
                                            $" / field type being set ({PropertyType}) using {nameof(ColumnConfig<T>)} for variable named '{DisplayName}'" +
                                            $" (Identifier - {Identifier}).", nameof(value));
            }

            _setter.Invoke(target, value);
            return target;
        }

        /// <summary>
        /// Extracts underlying value (value source is pointed to in source <see cref="ColumnConfig{T}"/>) from given <paramref name="source"/>.<br/>
        /// Values can be extracted only from properties or fields stored in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Object from which we are trying to get a value.</param>
        /// <returns>Value extracted from <paramref name="source"/>.</returns>
        public dynamic GetValueFrom(T source) => source is null ? null : _getter.Invoke(source);

        public bool IsValidatable(Type type = null)
        {
            if (type is null)
                return Validator is not null;
            return Validator?.CanValidateInstancesOfType(type) ?? false;
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