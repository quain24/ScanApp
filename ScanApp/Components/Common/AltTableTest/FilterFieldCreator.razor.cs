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
        private readonly Dictionary<Guid, Delegate> _cachedFromToGetValueDelegates = new();
        private readonly Dictionary<Guid, (Delegate From, Delegate To)> _cachedFromToValidationDelegates = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            FilterableConfigs = Configs.Where(c => c.IsFilterable);
            CacheFromToDelegates();
            CacheFromToValidators();
        }

        private RenderFragment CreateFilterField(ColumnConfig<T> config)
        {
            return builder =>
            {
                if (config.PropertyType.IsNumeric())
                    CreateFromToFields(builder, config);
                //else if ((config.PropertyType == typeof(DateTime?) || config.PropertyType == typeof(DateTime)) &&
                //         (config.FieldType is FieldType.AutoDetect or FieldType.Date or FieldType.DateAndTime))
                //    CreateFromToDateTimeFields(builder, config);
                ////if ((config.PropertyType == typeof(DateTime?) && config.FieldType == FieldType.AutoDetect) ||
                //    (config.PropertyType == typeof(TimeSpan?) && config.FieldType == FieldType.AutoDetect) ||
                //    config.FieldType is FieldType.Time or FieldType.DateAndTime)
                //    CreateTimeFields(builder, config);
                //else if (config.FieldType == FieldType.PlainText || (config.PropertyType != typeof(DateTime?) && config.PropertyType != typeof(TimeSpan?)))
                //    CreateTextField(builder, config);
            };
        }

        private static Type MakeNullable(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                ? type
                : typeof(Nullable<>).MakeGenericType(type);
        }

        private void CacheFromToDelegates()
        {
            foreach (var cfg in FilterableConfigs)
            {
                if (!cfg.PropertyType.IsNumeric())
                    continue;

                var valueType = MakeNullable(cfg.PropertyType);

                var param = Expression.Parameter(cfg.PropertyType);
                var body = Expression.Convert(param, valueType);
                var lambda = Expression.Lambda(body, param);

                _cachedFromToGetValueDelegates.Add(cfg.Identifier, lambda.Compile());
            }
        }

        private void CacheFromToValidators()
        {
            foreach (var cfg in FilterableConfigs)
            {
                if (!cfg.PropertyType.IsNumeric())
                    continue;

                var valueType = MakeNullable(cfg.PropertyType);
                var validatorFuncType = Expression.GetDelegateType(valueType, typeof(IEnumerable<string>));

                var methodTypeFrom = GetType().GetMethod("ValidateFrom", BindingFlags.NonPublic | BindingFlags.Instance)?.MakeGenericMethod(valueType);
                var validationFrom = Delegate.CreateDelegate(validatorFuncType, this, methodTypeFrom);
                var methodTypeTo = GetType().GetMethod("ValidateTo", BindingFlags.NonPublic | BindingFlags.Instance)?.MakeGenericMethod(valueType);
                var validationTo = Delegate.CreateDelegate(validatorFuncType, this, methodTypeTo);

                _cachedFromToValidationDelegates.Add(cfg.Identifier, (validationFrom, validationTo));
            }
        }

        private bool ValidateFromTo(dynamic from, dynamic to)
        {
            if (from is null || to is null)
                return true;
            return from <= to;
        }

        private Guid curKey;

        private IEnumerable<string> ValidateFrom<TNumeric>(TNumeric from)
        {
            if (from is null)
                return Array.Empty<string>();
            return (dynamic)@from > (TNumeric)_fromToValues[curKey].To ? new[] { "From must be lesser than to." } : Array.Empty<string>();
        }

        private IEnumerable<string> ValidateTo<TNumeric>(TNumeric to)
        {
            if (to is null)
                return Array.Empty<string>();
            return (dynamic)to < (TNumeric)_fromToValues[curKey].From ? new[] { "To must be greater than from." } : Array.Empty<string>();
        }

        private Dictionary<Guid, bool> _results = new();

        private void CreateFromToFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            _fromToValues.TryAdd(config.Identifier, (null, null));
            foreach (var kvp in _fromToValues)
            {
                bool res = ValidateFromTo(kvp.Value.From, kvp.Value.To);
                if (_results.TryAdd(kvp.Key, res))
                    continue;
                _results[kvp.Key] = res;
            }

            var valueType = MakeNullable(config.PropertyType);
            var fieldType = typeof(MudTextField<>).MakeGenericType(valueType);

            curKey = config.Identifier;
            // Create 'from'
            builder.OpenComponent(LineNumber.Get(), fieldType);
            builder.AddAttribute(LineNumber.Get(), "Value", _cachedFromToGetValueDelegates[config.Identifier].DynamicInvoke(GetData(config, true)));

            var callbackType = typeof(EventCallback<>).MakeGenericType(valueType);
            void SaveFromDelegate(dynamic obj) => _fromToValues[config.Identifier] = (obj, _fromToValues[config.Identifier].To);
            dynamic callbackFrom = Activator.CreateInstance(callbackType, this, (Action<dynamic>)SaveFromDelegate);
            builder.AddAttribute(LineNumber.Get(), "ValueChanged", callbackFrom);

            //builder.AddAttribute(LineNumber.Get(), "Validation", _cachedFromToValidationDelegates[config.Identifier].From);

            builder.AddAttribute(LineNumber.Get(), "Error", !_results[config.Identifier]);
            builder.AddAttribute(LineNumber.Get(), "ErrorText", _results[config.Identifier] ? string.Empty : "From must be lesser or equal to To.");

            builder.AddAttribute(LineNumber.Get(), "Label", "From");
            builder.AddAttribute(LineNumber.Get(), "Immediate", true);
            builder.CloseComponent();

            // Create 'To'
            builder.OpenComponent(LineNumber.Get(), fieldType);

            builder.AddAttribute(LineNumber.Get(), "Value", _cachedFromToGetValueDelegates[config.Identifier].DynamicInvoke(GetData(config, false)));
            void SaveToDelegate(dynamic obj)
            {
                _fromToValues[config.Identifier] = (_fromToValues[config.Identifier].From, obj);
            }
            dynamic callbackTo = Activator.CreateInstance(callbackType, this, (Action<dynamic>)SaveToDelegate);
            builder.AddAttribute(LineNumber.Get(), "ValueChanged", callbackTo);

            //builder.AddAttribute(LineNumber.Get(), "Validation", _cachedFromToValidationDelegates[config.Identifier].To);
            builder.AddAttribute(LineNumber.Get(), "Error", !_results[config.Identifier]);
            builder.AddAttribute(LineNumber.Get(), "ErrorText", _results[config.Identifier] ? string.Empty : "To must be greater or equal to From.");

            builder.AddAttribute(LineNumber.Get(), "Immediate", true);
            builder.AddAttribute(LineNumber.Get(), "Label", "To");
            builder.CloseComponent();
        }

        private object GetData(ColumnConfig<T> config, bool isFrom)
        {
            return isFrom ? _fromToValues[config.Identifier].From : _fromToValues[config.Identifier].To;
        }

        private void CreateFromToDateTimeFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            _fromToValues.TryAdd(config.Identifier, (config.PropertyType.GetDefaultValue(), config.PropertyType.GetDefaultValue()));

            // Create 'from'
            CreateDateField(builder, config, "From", true);
            CreateTimeField(builder, config, "From", true);

            // Create 'To'
            CreateDateField(builder, config, "To", false);
            CreateTimeField(builder, config, "To", false);
        }

        private void CreateDateField(RenderTreeBuilder builder, ColumnConfig<T> config, string label, bool isFrom)
        {
            var fieldType = typeof(MudDatePicker);
            Validators.TryGetValue(config, out var validatorDelegate);
            _fromToValues.TryAdd(config.Identifier, (config.PropertyType.GetDefaultValue(), config.PropertyType.GetDefaultValue()));

            var value = isFrom ? _fromToValues[config.Identifier].From : _fromToValues[config.Identifier].To;

            builder.OpenComponent(LineNumber.Get(), fieldType);
            builder.AddAttribute(LineNumber.Get(), "Date", value as DateTime?);

            var callback = CallbackFactory.Create<DateTime?>(this, d => EditDate(d, config, isFrom));
            builder.AddAttribute(LineNumber.Get(), "DateChanged", callback);

            if (validatorDelegate is not null)
                builder.AddAttribute(LineNumber.Get(), "Validation", validatorDelegate);

            builder.AddAttribute(LineNumber.Get(), "Label", label);
            builder.AddAttribute(LineNumber.Get(), "Immediate", true);
            builder.CloseComponent();
        }

        private void CreateTimeField(RenderTreeBuilder builder, ColumnConfig<T> config, string label, bool isFrom)
        {
            builder.OpenComponent(20, typeof(MudTimePicker));
            Validators.TryGetValue(config, out var validatorDelegate);
            _fromToValues.TryAdd(config.Identifier, (config.PropertyType.GetDefaultValue(), config.PropertyType.GetDefaultValue()));
            var value = isFrom ? _fromToValues[config.Identifier].From : _fromToValues[config.Identifier].To;

            if (config.PropertyType == typeof(DateTime?))
            {
                var data = value as DateTime?;
                TimeSpan? time = data.HasValue switch
                {
                    true => data.Value.TimeOfDay,
                    false => null
                };

                builder.AddAttribute(21, "Time", time);
                var callback = CallbackFactory.Create<TimeSpan?>(this, d => EditTimeInDate(d, config, isFrom));
                builder.AddAttribute(22, "TimeChanged", callback);
            }

            if (config.PropertyType == typeof(TimeSpan?))
            {
                builder.AddAttribute(21, "Time", value as TimeSpan?);
                var callback = CallbackFactory.Create<TimeSpan?>(this, d => EditTime(d, config, isFrom));
                builder.AddAttribute(22, "TimeChanged", callback);
            }

            if (validatorDelegate is not null)
                builder.AddAttribute(23, "Validation", validatorDelegate);

            builder.AddAttribute(LineNumber.Get(), "Label", label);
            builder.AddAttribute(24, "Immediate", true);
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