using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;
using ScanApp.Common.Extensions;
using ScanApp.Common.Helpers;
using ScanApp.Components.Common.ScanAppTable.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ScanApp.Components.Common.AltTableTest
{
    public partial class FilterFieldCreator<T> : FieldCreatorBase<T>
    {
        [Parameter]
        public IEnumerable<T> SourceCollection { get; set; }

        [Parameter] public IEnumerable<T> FilteredCollection { get; set; }

        [Parameter] public string FromLabel { get; set; } = "From";
        [Parameter] public string ToLabel { get; set; } = "To";
        [Parameter] public string ErrorMessageFromTo { get; set; }

        private IEnumerable<ColumnConfig<T>> FilterableConfigs { get; set; }
        private readonly Dictionary<Guid, (dynamic From, dynamic To)> _fromToValues = new();
        private readonly Dictionary<Guid, (dynamic From, dynamic To)> _fieldReferences = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            FilterableConfigs = Configs.Where(c => c.IsFilterable);
            ErrorMessageFromTo ??= $"'{FromLabel}' cannot be greater than '{ToLabel}'.";
        }

        private RenderFragment CreateFilterField(ColumnConfig<T> config)
        {
            return builder =>
            {
                if (config.PropertyType.IsNumeric())
                    CreateFromToFields(builder, config);
                else if ((config.PropertyType == typeof(DateTime?) || config.PropertyType == typeof(DateTime)) &&
                         (config.FieldType is FieldType.AutoDetect or FieldType.DateAndTime))
                    CreateFromToDateTimeFields(builder, config);
                else if ((config.PropertyType == typeof(DateTime?) || config.PropertyType == typeof(DateTime)) && config.FieldType == FieldType.Date)
                    CreateFromToDateFields(builder, config);
                else if ((config.PropertyType == typeof(DateTime?) || config.PropertyType == typeof(DateTime) || config.PropertyType == typeof(TimeSpan?) || config.PropertyType == typeof(TimeSpan)) &&
                         (config.FieldType is FieldType.AutoDetect or FieldType.Time))
                    CreateFromToTimeFields(builder, config);
            };
        }

        private void CreateFromToFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            _fromToValues.TryAdd(config.Identifier, (null, null));

            var valueType = EnsureNullable(config.PropertyType);
            var fieldType = typeof(MudNumericField<>).MakeGenericType(valueType);
            var readMethod = GetType().GetMethod(nameof(GetDataFromTo), BindingFlags.NonPublic | BindingFlags.Instance)?.MakeGenericMethod(valueType);
            var readDelegate = Delegate.CreateDelegate(Expression.GetFuncType(typeof(Guid), typeof(bool), valueType), this, readMethod);

            // Create 'from'
            builder.OpenComponent(LineNumber.Get(), fieldType);
            builder.AddAttribute(LineNumber.Get(), "Value", readDelegate.DynamicInvoke(config.Identifier, true));

            var callbackType = typeof(EventCallback<>).MakeGenericType(valueType);
            void SaveFromDelegate(dynamic obj) => _fromToValues[config.Identifier] = (obj, _fromToValues[config.Identifier].To);
            dynamic callbackFrom = Activator.CreateInstance(callbackType, this, (Action<dynamic>)SaveFromDelegate);
            builder.AddAttribute(LineNumber.Get(), "ValueChanged", callbackFrom);

            builder.AddAttribute(LineNumber.Get(), "Immediate", true);
            builder.AddAttribute(LineNumber.Get(), "Label", $"{config.DisplayName} - {FromLabel}");
            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config, true));
            builder.CloseComponent();

            // Create 'To'
            builder.OpenComponent(LineNumber.Get(), fieldType);

            builder.AddAttribute(LineNumber.Get(), "Value", readDelegate.DynamicInvoke(config.Identifier, false));
            void SaveToDelegate(dynamic obj) => _fromToValues[config.Identifier] = (_fromToValues[config.Identifier].From, obj);
            dynamic callbackTo = Activator.CreateInstance(callbackType, this, (Action<dynamic>)SaveToDelegate);
            builder.AddAttribute(LineNumber.Get(), "ValueChanged", callbackTo);

            builder.AddAttribute(LineNumber.Get(), "Immediate", true);
            builder.AddAttribute(LineNumber.Get(), "Validation", CreateValidationDelegate(config, valueType));
            builder.AddAttribute(LineNumber.Get(), "Label", $"{config.DisplayName} - {ToLabel}");
            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config, false));
            builder.CloseComponent();
        }

        private TData GetDataFromTo<TData>(Guid id, bool isFrom) => (isFrom ? _fromToValues[id].From : _fromToValues[id].To);

        private static Type EnsureNullable(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                ? type
                : typeof(Nullable<>).MakeGenericType(type);
        }

        private void CreateFieldReference(object o, ColumnConfig<T> config, bool isFrom)
        {
            if (_fieldReferences.TryGetValue(config.Identifier, out var value))
            {
                if (value.From is not null && value.To is not null)
                    return;
                _fieldReferences[config.Identifier] = isFrom switch
                {
                    true when value.From is null => (o, _fieldReferences[config.Identifier].To),
                    false when value.To is null => (_fieldReferences[config.Identifier].From, o),
                    _ => _fieldReferences[config.Identifier]
                };
            }
            else
            {
                _fieldReferences.Add(config.Identifier, isFrom ? (o, null) : (null, o));
            }
        }

        private Delegate CreateValidationDelegate(ColumnConfig<T> config, Type inputType)
        {
            bool isValid = _fromToValues[config.Identifier].From is null || _fromToValues[config.Identifier].To is null ||
                           _fromToValues[config.Identifier].From <= _fromToValues[config.Identifier].To;
            var inputParam = Expression.Parameter(inputType);
            var compatibilityCheckMethod = GetType()
                ?.GetMethod(nameof(AreTypesCompatible), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(inputType);
            var currentConfigParam = Expression.Constant(config, typeof(ColumnConfig<T>));
            var compatibilityCheckExpr = Expression.Call(compatibilityCheckMethod, inputParam, currentConfigParam);
            var isCompatibleExpr = Expression.IsTrue(compatibilityCheckExpr);
            var ifErrorExpr = Expression.Constant(ErrorMessageFromTo, typeof(string));
            var ifValidExpr = Expression.Constant(null, typeof(string));
            var isValidExpr = Expression.Constant(isValid, typeof(bool));
            var isValidAllExpr = Expression.IsTrue(Expression.And(isValidExpr, isCompatibleExpr));
            var body = Expression.Lambda(Expression.Condition(Expression.IsTrue(isValidAllExpr), ifValidExpr, ifErrorExpr), inputParam);
            return body.Compile();
        }

        private static bool AreTypesCompatible<TValueType>(TValueType val, ColumnConfig<T> cc)
        {
            var type = val?.GetType();
            return val is null || cc.PropertyType == type || Nullable.GetUnderlyingType(cc.PropertyType) == type;
        }

        private void CreateFromToDateTimeFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            _fromToValues.TryAdd(config.Identifier, (null, null));

            // Create 'from'
            CreateDateField(builder, config, true, true);
            CreateTimeField(builder, config, true);

            // Create 'To'
            CreateDateField(builder, config, false);
            CreateTimeField(builder, config, false);
        }

        private void CreateFromToDateFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            _fromToValues.TryAdd(config.Identifier, (null, null));
            CreateDateField(builder, config, true, true);
            CreateDateField(builder, config, false);
        }

        private void CreateFromToTimeFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            _fromToValues.TryAdd(config.Identifier, (null, null));
            CreateTimeField(builder, config, true, true);
            CreateTimeField(builder, config, false);
        }

        private void CreateDateField(RenderTreeBuilder builder, ColumnConfig<T> config, bool isFrom, bool shouldValidate = false)
        {
            var fieldType = typeof(MudDatePicker);

            DateTime? value = isFrom ? _fromToValues[config.Identifier].From : _fromToValues[config.Identifier].To;

            builder.OpenComponent(LineNumber.Get(), fieldType);
            builder.AddAttribute(LineNumber.Get(), "Date", value);

            var callback = CallbackFactory.Create<DateTime?>(this, d => EditDate(d, config, isFrom));
            builder.AddAttribute(LineNumber.Get(), "DateChanged", callback);

            if (shouldValidate)
            {
                string ValidateDateFromTo(DateTime? date)
                {
                    if (date is null) return null;
                    if (isFrom && ((DateTime?)_fromToValues[config.Identifier].To).HasValue is false) return null;
                    if (isFrom is false && ((DateTime?)_fromToValues[config.Identifier].From).HasValue is false) return null;
                    var compareTo = isFrom ? (DateTime?)_fromToValues[config.Identifier].To.Date : (DateTime?)_fromToValues[config.Identifier].From.Date;
                    return isFrom ? (date <= compareTo ? null : "error") : (date >= compareTo ? null : "error");
                }

                builder.AddAttribute(LineNumber.Get(), "Validation", (Func<DateTime?, string>)ValidateDateFromTo);
            }

            builder.AddAttribute(LineNumber.Get(), "Label", isFrom ? FromLabel : ToLabel);
            builder.AddAttribute(LineNumber.Get(), "Immediate", true);
            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config, isFrom));
            builder.CloseComponent();
        }

        private void CreateTimeField(RenderTreeBuilder builder, ColumnConfig<T> config, bool isFrom, bool shouldValidate = false)
        {
            builder.OpenComponent(LineNumber.Get(), typeof(MudTimePicker));
            var value = isFrom ? _fromToValues[config.Identifier].From : _fromToValues[config.Identifier].To;

            if (config.PropertyType == typeof(DateTime?))
            {
                var data = value as DateTime?;
                TimeSpan? time = data.HasValue switch
                {
                    true => data.Value.TimeOfDay,
                    false => null
                };

                builder.AddAttribute(LineNumber.Get(), "Time", time);
                var callback = CallbackFactory.Create<TimeSpan?>(this, d => EditTimeInDate(d, config, isFrom));
                builder.AddAttribute(LineNumber.Get(), "TimeChanged", callback);
                if (shouldValidate)
                {
                    string ValidateDateFromTo(TimeSpan? time)
                    {
                        if (time is null) return null;
                        if (isFrom && ((DateTime?)_fromToValues[config.Identifier].To).HasValue is false) return null;
                        if (isFrom is false && ((DateTime?)_fromToValues[config.Identifier].From).HasValue is false) return null;

                        var compareTo = isFrom ? (TimeSpan?)_fromToValues[config.Identifier].To.TimeOfDay : (TimeSpan?)_fromToValues[config.Identifier].From.TimeOfDay;
                        return isFrom ? (time <= compareTo ? null : "error") : (time >= compareTo ? null : "error");
                    }

                    builder.AddAttribute(LineNumber.Get(), "Validation", (Func<TimeSpan?, string>)ValidateDateFromTo);
                }
            }

            if (config.PropertyType == typeof(TimeSpan?))
            {
                builder.AddAttribute(LineNumber.Get(), "Time", value as TimeSpan?);
                var callback = CallbackFactory.Create<TimeSpan?>(this, d => EditTime(d, config, isFrom));
                builder.AddAttribute(LineNumber.Get(), "TimeChanged", callback);
                if (shouldValidate)
                {
                    string ValidateDateFromTo(TimeSpan? date)
                    {
                        if (date is null) return null;
                        var compareTo = isFrom ? (TimeSpan?)_fromToValues[config.Identifier].To : (TimeSpan?)_fromToValues[config.Identifier].From;
                        return isFrom ? (date <= compareTo ? null : "error") : (date >= compareTo ? null : "error");
                    }

                    builder.AddAttribute(LineNumber.Get(), "Validation", (Func<TimeSpan?, string>)ValidateDateFromTo);
                }
            }

            builder.AddAttribute(LineNumber.Get(), "Label", isFrom ? FromLabel : ToLabel);
            builder.AddAttribute(LineNumber.Get(), "Immediate", true);
            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config, isFrom));

            builder.CloseComponent();
        }

        private void EditDate(DateTime? date, ColumnConfig<T> config, bool from)
        {
            var oldDate = (from ? _fromToValues[config.Identifier].From : _fromToValues[config.Identifier].To) as DateTime?;
            if (date != oldDate && oldDate.HasValue)
            {
                date += oldDate.Value.TimeOfDay;
            }

            if (from)
            {
                _fromToValues[config.Identifier] = (date, _fromToValues[config.Identifier].To);
            }
            else
            {
                _fromToValues[config.Identifier] = (_fromToValues[config.Identifier].From, date);
            }
        }

        private void EditTimeInDate(TimeSpan? newTime, ColumnConfig<T> config, bool isFrom)
        {
            var source = isFrom ? _fromToValues[config.Identifier].From : _fromToValues[config.Identifier].To;

            if (source is DateTime oldDate)
            {
                var newDate = oldDate.Date + newTime;
                if (newDate != oldDate)
                {
                    if (isFrom)
                    {
                        _fromToValues[config.Identifier] = (newDate, _fromToValues[config.Identifier].To);
                    }
                    else
                    {
                        _fromToValues[config.Identifier] = (_fromToValues[config.Identifier].From, newDate);
                    }
                }
            }
            else if (newTime.HasValue)
            {
                if (isFrom)
                {
                    _fromToValues[config.Identifier] = (DateTime.MinValue + newTime.Value, _fromToValues[config.Identifier].To);
                }
                else
                {
                    _fromToValues[config.Identifier] = (_fromToValues[config.Identifier].From, DateTime.MinValue + newTime.Value);
                }
            }
        }

        private void EditTime(TimeSpan? time, ColumnConfig<T> config, bool isFrom)
        {
            var source = (isFrom ? _fromToValues[config.Identifier].From : _fromToValues[config.Identifier].To) as TimeSpan?;

            if (time != source)
            {
                if (isFrom)
                {
                    _fromToValues[config.Identifier] = (DateTime.MinValue + time.Value, _fromToValues[config.Identifier].To);
                }
                else
                {
                    _fromToValues[config.Identifier] = (_fromToValues[config.Identifier].From, DateTime.MinValue + time.Value);
                }
            }
        }
    }
}