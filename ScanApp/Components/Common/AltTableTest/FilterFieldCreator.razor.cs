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

        private IEnumerable<ColumnConfig<T>> FilterableConfigs { get; set; }
        private readonly Dictionary<Guid, (dynamic From, dynamic To)> _fromToValues = new();
        private readonly Dictionary<Guid, (dynamic From, dynamic To)> _fieldReferences = new();
        private readonly Dictionary<Guid, Delegate> _cachedFromToGetValueDelegates = new();
        private readonly Dictionary<Guid, (Delegate From, Delegate To)> _cachedFromToValidationDelegates = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            FilterableConfigs = Configs.Where(c => c.IsFilterable);
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
                ////if ((config.PropertyType == typeof(DateTime?) && config.FieldType == FieldType.AutoDetect) ||
                //    (config.PropertyType == typeof(TimeSpan?) && config.FieldType == FieldType.AutoDetect) ||
                //    config.FieldType is FieldType.Time or FieldType.DateAndTime)
                //    CreateTimeFields(builder, config);
                //else if (config.FieldType == FieldType.PlainText || (config.PropertyType != typeof(DateTime?) && config.PropertyType != typeof(TimeSpan?)))
                //    CreateTextField(builder, config);
            };
        }

        private void CreateFromToFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            _fromToValues.TryAdd(config.Identifier, (null, null));

            var valueType = MakeNullable(config.PropertyType);
            var fieldType = typeof(MudNumericField<>).MakeGenericType(valueType);
            var readMethod = GetType().GetMethod(nameof(GetData), BindingFlags.NonPublic | BindingFlags.Instance)?.MakeGenericMethod(valueType);
            var readDelegate = Delegate.CreateDelegate(Expression.GetFuncType(typeof(Guid), typeof(bool), valueType), this, readMethod);

            // Create 'from'
            builder.OpenComponent(LineNumber.Get(), fieldType);
            builder.AddAttribute(LineNumber.Get(), "Value", readDelegate.DynamicInvoke(config.Identifier, true));

            var callbackType = typeof(EventCallback<>).MakeGenericType(valueType);
            void SaveFromDelegate(dynamic obj) => _fromToValues[config.Identifier] = (obj, _fromToValues[config.Identifier].To);
            dynamic callbackFrom = Activator.CreateInstance(callbackType, this, (Action<dynamic>)SaveFromDelegate);
            builder.AddAttribute(LineNumber.Get(), "ValueChanged", callbackFrom);

            builder.AddAttribute(LineNumber.Get(), "Immediate", true);
            builder.AddAttribute(LineNumber.Get(), "Label", $"{config.DisplayName} - From");
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
            builder.AddAttribute(LineNumber.Get(), "Label", $"{config.DisplayName} - To");
            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config, false));
            builder.CloseComponent();
        }

        private TData GetData<TData>(Guid id, bool isFrom) => (isFrom ? _fromToValues[id].From : _fromToValues[id].To);

        private static Type MakeNullable(Type type)
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
            var isValid = inputType switch
            {
                var t when t == typeof(DateTime?) && _fieldReferences.ContainsKey(config.Identifier) =>
                    _fieldReferences[config.Identifier].From?.Date is null ||
                    _fieldReferences[config.Identifier].To?.Date is null ||
                    _fieldReferences[config.Identifier].From?.Date <= _fieldReferences[config.Identifier].To?.Date,
                var t when t == typeof(TimeSpan?) && _fieldReferences.ContainsKey(config.Identifier) =>
                    _fieldReferences[config.Identifier].From?.Time is null ||
                    _fieldReferences[config.Identifier].To?.Time is null ||
                    _fieldReferences[config.Identifier].From?.Time <= _fieldReferences[config.Identifier].To?.Time,
                _ when _fieldReferences.ContainsKey(config.Identifier) is false => true,
                _ => _fieldReferences[config.Identifier].From?.Value is null ||
                     _fieldReferences[config.Identifier].To?.Value is null ||
                     _fieldReferences[config.Identifier].From?.Value <= _fieldReferences[config.Identifier].To?.Value
            };

            var inputParam = Expression.Parameter(inputType);
            var compatibilityCheckMethod = GetType()
                ?.GetMethod(nameof(AreTypesCompatible), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(inputType);
            var currentConfigParam = Expression.Constant(config, typeof(ColumnConfig<T>));
            var compatibilityCheckExpr = Expression.Call(compatibilityCheckMethod, inputParam, currentConfigParam);
            var isCompatibleExpr = Expression.IsTrue(compatibilityCheckExpr);
            var ifErrorExpr = Expression.Constant(new[] { "'From' must be greater or equal to 'To' or one of fields should be empty." }, typeof(IEnumerable<string>));
            var ifValidExpr = Expression.Constant(Array.Empty<string>(), typeof(IEnumerable<string>));
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
            CreateDateField(builder, config, "From", true, true);
            CreateTimeField(builder, config, "From", true);

            // Create 'To'
            CreateDateField(builder, config, "To", false);
            CreateTimeField(builder, config, "To", false);
        }

        private void CreateDateField(RenderTreeBuilder builder, ColumnConfig<T> config, string label, bool isFrom, bool shouldValidate = false)
        {
            var fieldType = typeof(MudDatePicker);

            var value = isFrom ? _fromToValues[config.Identifier].From : _fromToValues[config.Identifier].To;

            builder.OpenComponent(LineNumber.Get(), fieldType);
            builder.AddAttribute(LineNumber.Get(), "Date", value as DateTime?);

            var callback = CallbackFactory.Create<DateTime?>(this, d => EditDate(d, config, isFrom));
            builder.AddAttribute(LineNumber.Get(), "DateChanged", callback);

            if (shouldValidate)
                builder.AddAttribute(LineNumber.Get(), "Validation", CreateValidationDelegate(config, typeof(DateTime?)));

            builder.AddAttribute(LineNumber.Get(), "Label", label);
            builder.AddAttribute(LineNumber.Get(), "Immediate", true);
            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config, isFrom));
            builder.CloseComponent();
        }

        private void CreateTimeField(RenderTreeBuilder builder, ColumnConfig<T> config, string label, bool isFrom, bool shouldValidate = false)
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
                    builder.AddAttribute(LineNumber.Get(), "Validation", CreateValidationDelegate(config, typeof(DateTime?)));
            }

            if (config.PropertyType == typeof(TimeSpan?))
            {
                builder.AddAttribute(LineNumber.Get(), "Time", value as TimeSpan?);
                var callback = CallbackFactory.Create<TimeSpan?>(this, d => EditTime(d, config, isFrom));
                builder.AddAttribute(LineNumber.Get(), "TimeChanged", callback);
                if (shouldValidate)
                    builder.AddAttribute(LineNumber.Get(), "Validation", CreateValidationDelegate(config, typeof(TimeSpan?)));
            }

            builder.AddAttribute(LineNumber.Get(), "Label", label);
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

        //private void CreateTextField(RenderTreeBuilder builder, ColumnConfig<T> config)
        //{
        //    // Type of mud blazor text field (int, string, etc)
        //    var textFieldType = typeof(MudTextField<>).MakeGenericType(config.PropertyType);

        //    // Start creating text field
        //    builder.OpenComponent(0, textFieldType);
        //    builder.AddAttribute(1, "Value", config.GetValueFrom(SourceCollection) as object);

        //    // Set callback for edit action
        //    var callbackType = typeof(EventCallback<>).MakeGenericType(config.PropertyType);
        //    async Task EditDelegate(dynamic obj)
        //    {
        //        ColumnConfigExtensions.SetValue(config, SourceCollection, obj);
        //        await TargetItemChanged.InvokeAsync(SourceCollection);
        //    }

        //    dynamic callback = Activator.CreateInstance(callbackType, this, (Func<dynamic, Task>)EditDelegate);
        //    builder.AddAttribute(2, "ValueChanged", callback);

        //    // Set up corresponding validator
        //    if (Validators.TryGetValue(config, out var validatorDelegate))
        //        builder.AddAttribute(3, "Validation", validatorDelegate);

        //    // Set common options
        //    builder.AddAttribute(4, "Immediate", true);
        //    builder.AddAttribute(5, "Disabled", !config.IsEditable);

        //    // Finish component
        //    builder.CloseComponent();
        //}

        //private void CreateDateFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        //{
        //    builder.OpenComponent(10, typeof(MudDatePicker));
        //    builder.AddAttribute(11, "Date", config.GetValueFrom(SourceCollection) as DateTime?);

        //    var callback = CallbackFactory.Create<DateTime?>(this, d => EditDate(d, SourceCollection, config));
        //    builder.AddAttribute(12, "DateChanged", callback);

        //    if (Validators.TryGetValue(config, out var validatorDelegate))
        //        builder.AddAttribute(13, "Validation", validatorDelegate);

        //    builder.AddAttribute(14, "Immediate", true);
        //    builder.AddAttribute(15, "Disabled", !config.IsEditable);

        //    builder.CloseComponent();
        //}

        //private void CreateTimeFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        //{
        //    builder.OpenComponent(20, typeof(MudTimePicker));

        //    if (config.PropertyType == typeof(DateTime?))
        //    {
        //        var data = config.GetValueFrom(SourceCollection) as DateTime?;
        //        TimeSpan? time = data.HasValue switch
        //        {
        //            true => data.Value.TimeOfDay,
        //            false => null
        //        };

        //        builder.AddAttribute(21, "Time", time);
        //        var callback = CallbackFactory.Create<TimeSpan?>(this, d => EditTimeInDate(d, SourceCollection, config));
        //        builder.AddAttribute(22, "TimeChanged", callback);
        //    }

        //    if (config.PropertyType == typeof(TimeSpan?))
        //    {
        //        var time = config.GetValueFrom(SourceCollection) as TimeSpan?;
        //        builder.AddAttribute(21, "Time", time);
        //        var callback = CallbackFactory.Create<TimeSpan?>(this, d => EditTime(d, SourceCollection, config));
        //        builder.AddAttribute(22, "TimeChanged", callback);
        //    }

        //    if (Validators.TryGetValue(config, out var validatorDelegate))
        //        builder.AddAttribute(23, "Validation", validatorDelegate);

        //    builder.AddAttribute(24, "Immediate", true);
        //    builder.AddAttribute(25, "Disabled", !config.IsEditable);

        //    builder.CloseComponent();
        //}

        //private async Task EditDate(DateTime? date, T target, ColumnConfig<T> config)
        //{
        //    var oldDate = config.GetValueFrom(SourceCollection) as DateTime?;
        //    if (date != oldDate && oldDate.HasValue)
        //    {
        //        date += oldDate.Value.TimeOfDay;
        //    }
        //    config.SetValue(SourceCollection, date);
        //    await TargetItemChanged.InvokeAsync(SourceCollection);
        //}

        //private async Task EditTime(TimeSpan? time, T target, ColumnConfig<T> config)
        //{
        //    var oldTime = config.GetValueFrom(SourceCollection) as TimeSpan?;

        //    if (time != oldTime)
        //    {
        //        config.SetValue(SourceCollection, time);
        //        await TargetItemChanged.InvokeAsync(SourceCollection);
        //    }
        //}

        //private async Task EditTimeInDate(TimeSpan? newTime, T target, ColumnConfig<T> config)
        //{
        //    if (config.GetValueFrom(SourceCollection) is DateTime oldDate)
        //    {
        //        var newDate = oldDate.Date + newTime;
        //        if (newDate != oldDate)
        //        {
        //            config.SetValue(SourceCollection, newDate);
        //            await TargetItemChanged.InvokeAsync(SourceCollection);
        //        }
        //    }
        //    else if (newTime.HasValue)
        //    {
        //        config.SetValue(SourceCollection, DateTime.MinValue + newTime.Value);
        //        await TargetItemChanged.InvokeAsync(SourceCollection);
        //    }
        //}
    }
}