using FluentValidation;
using FluentValidation.Internal;
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
    /// <summary>
    /// Represents a single column configuration for one property / method / field that will be used in a <see cref="SCTable{TTableType}"/>.<br/>
    /// Each <see cref="ColumnConfig{T}"/> points to one of properties / methods / fields from used type <typeparamref name="T"/> and contains all<br/>
    /// of configurations for <see cref="SCTable{TTableType}"/> instance, so it can display / filter / sort / etc given <typeparamref name="T"/> data collection.
    /// <para>
    /// <see cref="ColumnConfig{T}"/> is also responsible for getting and setting value pointed to by <br/>
    /// target set in it's instance.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type configured by this <see cref="ColumnConfig{T}"/> instance, used as data source for <see cref="SCTable{TTableType}"/>.</typeparam>
    public class ColumnConfig<T>
    {
        /// <summary>
        /// Gets this <see cref="ColumnConfig{T}"/> unique identifier.
        /// </summary>
        /// <value><see cref="Guid"/> identifier.</value>
        public Guid Identifier { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets name used as display name of one of <see cref="SCTable{TTableType}"/> columns.<br/>
        /// </summary>
        /// <value><see cref="string"/> representing column name displayed to user is set; Otherwise name of object that this instance is pointing to.</value>
        public string DisplayName { get; }

        /// <summary>
        /// Gets name of property to which this instance of <see cref="ColumnConfig{T}"/> is pointing.
        /// </summary>
        /// <value><see cref="string"/> representation of property / field / object name.</value>
        public string PropertyName { get; }

        /// <summary>
        /// Gets <see cref="Type"/> object that is pointed to by this instance of <see cref="ColumnConfig{T}"/>.
        /// </summary>
        /// <value><see cref="Type"/> of object targeted by this <see cref="ColumnConfig{T}"/>.</value>
        public Type PropertyType { get; }

        /// <summary>
        /// Gets type as which object targeted by this instance if <see cref="ColumnConfig{T}"/> should be displayed.
        /// </summary>
        /// <value>
        /// <see cref="FieldType"/> set when creating this instance , otherwise default (<see cref="FieldType.AutoDetect"/>).
        /// </value>
        public FieldType FieldType { get; }

        /// <summary>
        /// Gets converter used to convert between given value actual form and table-friendly display form of object pointed by this instance.
        /// </summary>
        /// <value><see cref="MudBlazor.Converter{T,U}"/> or <see cref="Converter{T}"/> as <see langword="dynamic"/> type value.</value>
        public dynamic Converter { get; private set; }

        /// <summary>
        /// Gets a value indicating if table using this <see cref="ColumnConfig{T}"/> can filter data by objects pointed to by this instance.<para/>
        /// By default this is set to <see langword="true"/>.
        /// </summary>
        /// <value><see langword="true"/> if table can be filtered using this instance of <see cref="ColumnConfig{T}"/>, otherwise <see langword="false"/>.</value>
        public bool IsFilterable { get; init; } = true;

        /// <summary>
        /// Gets a value indicating if table using this <see cref="ColumnConfig{T}"/> can edit value that it is pointing to.
        /// </summary>
        /// <value><see langword="true"/> if table can be edit data pointed to by this <see cref="ColumnConfig{T}"/>, otherwise <see langword="false"/>.</value>
        public bool IsEditable { get; init; }

        /// <summary>
        /// Gets a value indicating if table using this <see cref="ColumnConfig{T}"/> can group data by objects pointed to by this instance.
        /// </summary>
        /// <value><see langword="true"/> if table can be grouped by using this instance of <see cref="ColumnConfig{T}"/>, otherwise <see langword="false"/>.</value>
        public bool IsGroupable { get; init; }

        /// <summary>
        /// Gets or sets custom CSS style to be used when displaying table column configured by this instance.
        /// </summary>
        /// <value>A <see cref="string"/> representation of CSS style if set, otherwise <see langword="null"/>.</value>>
        public string ColumnStyle { get; init; }

        /// <summary>
        /// Gets or sets value indicating whether this instance is a 'Presenter' Column Config. if <see langword="true"/>,<br/>
        /// than it will not be bind to any presented values nor will it be able to display anything on its own - it has to be paired with corresponding <see cref="SCColumn{T}"/>.
        /// </summary>
        /// <value>if <see langword="true"/>, then this instance of <see cref="ColumnConfig{T}"/> is only a 'presenter' and will not display anything on its own.</value>
        public bool IsPresenter { get; }

        public IEnumerable<dynamic> AllowedValues { get; private set; } = Enumerable.Empty<dynamic>();
        private IValidator Validator { get; }
        private IReadOnlyList<MemberInfo> PathToItem { get; }
        private Expression<Func<T, dynamic>> Target { get; }
        private Func<T, dynamic> _getter;
        private Action<T, dynamic> _setter;
        private Func<T, dynamic, T> _valueSetter;

        /// <summary>
        /// Creates new instance of <see cref="ColumnConfig{T}"/> configured as a 'Presenter column' that should be paired with <see cref="SCColumn{T}"/>.<br/>
        /// A presenter <see cref="ColumnConfig{T}"/> does not enable editing for any value that corresponding <see cref="SCColumn{T}"/> will display.<para/>
        /// Typical use for this object is to facilitate display of additional buttons, images, external content without need to bind such content to one of values inside of <typeparamref name="T"/><br/>
        /// displayed by <see cref="SCTable{TTableType}"/>
        /// </summary>
        /// <param name="displayName">Name under which column configured in this instance will be displayed (name of column).</param>
        public ColumnConfig(string displayName)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            IsPresenter = true;
        }

        /// <summary>
        /// Creates new instance of <see cref="ColumnConfig{T}"/> configured given <paramref name="target"/>.
        /// </summary>
        /// <param name="target">Parameter / field / method that this <see cref="ColumnConfig{T}"/> will configure.</param>
        public ColumnConfig(Expression<Func<T, dynamic>> target)
            : this(target, null, FieldType.AutoDetect, null)
        {
        }

        ///<inheritdoc cref="ColumnConfig{T}(Expression{Func{T, dynamic}})"/>
        /// <param name="target">Parameter / field / method that this <see cref="ColumnConfig{T}"/> will configure.</param>
        /// <param name="displayName">Name under which <paramref name="target"/> will be displayed (name of column).</param>
        public ColumnConfig(Expression<Func<T, dynamic>> target, string displayName)
            : this(target, displayName, FieldType.AutoDetect, null)
        {
        }

        /// <inheritdoc cref="ColumnConfig{T}(Expression{Func{T, dynamic}})"/>
        /// <param name="target">Parameter / field / method that this <see cref="ColumnConfig{T}"/> will configure.</param>
        /// <param name="displayName">Name under which <paramref name="target"/> will be displayed (name of column).</param>
        /// <param name="validator">Validates elements pointed to by <paramref name="target"/>.</param>
        public ColumnConfig(Expression<Func<T, dynamic>> target, string displayName, IValidator validator)
            : this(target, displayName, FieldType.AutoDetect, validator)
        {
        }

        /// <inheritdoc cref="ColumnConfig{T}(Expression{Func{T, dynamic}})"/>
        /// <param name="target">Parameter / field / method that this <see cref="ColumnConfig{T}"/> will configure.</param>
        /// <param name="displayName">Name under which <paramref name="target"/> will be displayed (name of column).</param>
        /// <param name="format">Set display mode of object pointed to by <paramref name="target"/> (if possible).</param>
        public ColumnConfig(Expression<Func<T, dynamic>> target, string displayName, FieldType format)
            : this(target, displayName, format, null)
        {
        }

        /// <inheritdoc cref="ColumnConfig{T}(Expression{Func{T, dynamic}})"/>
        /// <param name="target">Parameter / field / method that this <see cref="ColumnConfig{T}"/> will configure.</param>
        /// <param name="displayName">Name under which <paramref name="target"/> will be displayed (name of column).</param>
        /// <param name="format">Set display mode of object pointed to by <paramref name="target"/> (if possible).</param>
        /// <param name="validator">Validates elements pointed to by <paramref name="target"/>.</param>
        public ColumnConfig(Expression<Func<T, dynamic>> target, string displayName, FieldType format, IValidator validator)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            PathToItem = PropertyPath<T>.GetFrom(Target);

            PropertyName = ExtractPropertyName();
            PropertyType = ExtractPropertyType();
            DisplayName = SetDisplayName(displayName);

            ChooseSetValueVersion();
            CreatePrecompiledSetterForItem();
            CreatePrecompiledGetterForItem();
            FieldType = format;
            Validator = SetUpValidator(validator);
        }

        /// <summary>
        /// Extracts underlying value (value source is pointed to in source <see cref="ColumnConfig{T}"/>) from given <paramref name="source"/>.<br/>
        /// Values can be extracted only from properties or fields stored in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Object from which we are trying to get a value.</param>
        /// <returns>Value extracted from <paramref name="source"/>.</returns>
        public dynamic GetValueFrom(T source) => source is null ? null : _getter.Invoke(source);

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

        private void ChooseSetValueVersion()
        {
            _valueSetter = PathToItem switch
            {
                var p when p.Count == 0 => SetValueDirect,
                var p when p.Count == 1 && typeof(T).IsValueType => TriedSetImmutableValue,
                var p when p[^1].ReflectedType?.IsValueType ?? true => TriedSetImmutableValue,
                var p when p[^1].IsWritable() is false => TriedSetImmutableValue,
                _ => SetValueWhenValid
            };
        }

        private static T SetValueDirect(T _, dynamic value) => value;

        private T TriedSetImmutableValue(T _, dynamic __) => throw new ArgumentException("Cannot write in immutable type or set method." +
                                                                                         $" - Identifier - '{Identifier}' | display name - '{DisplayName}'");

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

        private void CreatePrecompiledGetterForItem() => _getter = Target.Compile();

        private void CreatePrecompiledSetterForItem()
        {
            // No setter if target of this Column config points to something non-settable.
            if (_valueSetter != SetValueWhenValid)
                return;

            var valueParameterExpression = Expression.Parameter(typeof(object));
            var targetExpression = Target.Body is UnaryExpression unaryExpression ? unaryExpression.Operand : Target.Body;

            var assign = Expression.Lambda<Action<T, dynamic>>
            (
                Expression.Assign(targetExpression,
                    Expression.Convert(valueParameterExpression, targetExpression.Type)),
                Target.Parameters.Single(),
                valueParameterExpression
            );

            _setter = assign.Compile();
        }

        private IValidator SetUpValidator(IValidator validator)
        {
            if (validator is null)
                return null;

            if (validator.CanValidateInstancesOfType(PropertyType) is false)
            {
                throw new ArgumentException($"Given validator cannot validate field/property of type '{PropertyType.FullName}'" +
                                            $" pointed to by this {nameof(ColumnConfig<T>)} - GUID - {Identifier} | Property name - {PropertyName}.");
            }

            foreach (var validationRule in validator.CreateDescriptor()?.Rules ?? Enumerable.Empty<IValidationRule>())
            {
                validationRule.PropertyName = DisplayName;
            }

            return validator;
        }

        /// <summary>
        /// Informs if this instance of <see cref="ColumnConfig{T}"/> contains validator for it's target if no <paramref name="type"/> is provided.<br/>
        /// Otherwise checks if this instance contains validator capable of validating given <param name="type">.</param>
        /// </summary>
        /// <param name="type">Type being check for being compatible with validator (if any) used in this <see cref="ColumnConfig{T}"/>.</param>
        /// <returns>
        /// <para>
        ///     If no <paramref name="type"/> is given - returns <see langword="true"/> if this instance contains validator, otherwise returns <see langword="false"/>.
        /// </para>
        /// <para>
        ///     If <paramref name="type"/> is provided - returns <see langword="true"/> if it can be validated by validator contained
        ///     in this <see cref="ColumnConfig{T}"/>, otherwise returns <see langword="false"/>.
        /// </para>
        /// </returns>
        public bool IsValidatable(Type type = null)
        {
            if (type is null)
                return Validator is not null;
            return Validator?.CanValidateInstancesOfType(type) ?? false;
        }

        /// <summary>
        /// Validates given object.
        /// </summary>
        /// <typeparam name="TValueType">Type of validated object, must be compatible with validator contained by this instance.</typeparam>
        /// <param name="value">Object to be validated.</param>
        /// <returns>
        /// <see cref="IEnumerable{string}"/> containing all errors if there were any, otherwise <see cref="Enumerable.Empty{TValueType}"/> where TResult is <typeparamref name="TValueType"/>.
        /// </returns>
        /// <exception cref="ArgumentException">This instance of <see cref="ColumnConfig{T}"/> does not contain any validator.</exception>
        /// <exception cref="InvalidOperationException">Tried to validate value incompatible with validator inside this instance.</exception>
        public IEnumerable<string> Validate<TValueType>(TValueType value)
        {
            if (Validator is null)
            {
                throw new ArgumentException("Cannot validate when there is no validator set - " +
                                            "perhaps editing field tried to use this config as one with validation?");
            }
            var context = new ValidationContext<TValueType>(value, new PropertyChain(new[] { PropertyName }), new DefaultValidatorSelector());
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

        /// <summary>
        /// Assigns converter used to transform values pointed to by this <see cref="ColumnConfig{T}"/> to and from <see cref="SCTable{TTableType}"/> friendly form.
        /// </summary>
        /// <typeparam name="TType">Type of value to be converted - must be compatible with type of target set in this <see cref="ColumnConfig{T}"/>.</typeparam>
        /// <param name="converter">Converts value.</param>
        /// <returns>This instance of <see cref="ColumnConfig{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Type of converter is incompatible with type of target pointed by this instance of <see cref="ColumnConfig{T}"/>.</exception>
        public ColumnConfig<T> AssignConverter<TType>(Converter<TType> converter)
        {
            if (typeof(TType) != PropertyType)
            {
                throw new ArgumentException($"Given converter does not output compatible type (property - {PropertyType.FullName}), converter - {typeof(TType).FullName})");
            }
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
            return this;
        }

        public ColumnConfig<T> LimitAcceptedValuesTo<TType>(IEnumerable<TType> values)
        {
            if (typeof(TType).IsAssignableTo(PropertyType) is false)
            {
                throw new ArgumentException($"Given values are not compatible with target pointed to by this instance of {nameof(ColumnConfig<T>)}: (property - {PropertyType.FullName}), value collection - {typeof(TType).FullName})");
            }

            AllowedValues = (IEnumerable<dynamic>)values ?? throw new ArgumentNullException(nameof(values));
            return this;
        }
    }

    /// <summary>
    /// Display form.
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// Display form will be detected by <see cref="SCTable{TTableType}"/>.
        /// </summary>
        AutoDetect = 0,

        /// <summary>
        /// Data will be displayed as Date-only (in case of <see cref="DateTime"/> values and similar).
        /// </summary>
        Date,

        /// <summary>
        /// Data will be displayed as Time-only (in case of <see cref="DateTime"/> values and similar).
        /// </summary>
        Time,

        /// <summary>
        /// Data will be displayed as date and time, even if field does not contain data information.
        /// </summary>
        DateAndTime,

        /// <summary>
        /// A basic <c>.ToString()</c> method will be used when displaying data.
        /// </summary>
        PlainText
    }
}