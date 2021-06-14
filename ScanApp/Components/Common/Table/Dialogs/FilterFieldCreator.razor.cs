using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using ScanApp.Common.Extensions;
using ScanApp.Common.Helpers;
using ScanApp.Components.Common.Table.Utilities;

namespace ScanApp.Components.Common.Table.Dialogs
{
    public partial class FilterFieldCreator<T> : FieldCreatorBase<T>
    {
        [Parameter]
        public IEnumerable<T> SourceCollection { get; set; }

        [Parameter] public string FromLabel { get; set; } = "From";
        [Parameter] public string ToLabel { get; set; } = "To";
        [Parameter] public string IncludeLabel { get; set; } = "Must include...";
        [Parameter] public string ErrorMessageFromTo { get; set; }

        private readonly Dictionary<Guid, (dynamic From, dynamic To)> _fromToValues = new();
        private readonly Dictionary<Guid, (dynamic From, dynamic To)> _fieldReferencesFromTo = new();
        private readonly Dictionary<Guid, string> _includingValues = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ErrorMessageFromTo ??= $"'{FromLabel}' cannot be greater than '{ToLabel}'.";
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Configs = Configs.Where(c => c.IsFilterable);
        }

        public IEnumerable<IFilter<T>> GetFilters()
        {
            List<IFilter<T>> filters = new(_fromToValues.Count + _includingValues.Count);
            foreach (var (key, values) in _fromToValues)
            {
                var config = Configs.First(c => c.Identifier == key);

                if (values.From is null && values.To is null)
                    continue;

                IFilter<T> filter = values switch
                {
                    (null or DateTime, null or DateTime) v when config.FieldType is FieldType.Date =>
                        new InBetweenInclusiveFilterDateOnly<T>(config, v.From, v.To),
                    (null or DateTime, null or DateTime) v when config.FieldType is FieldType.Time =>
                        new InBetweenInclusiveFilterTimeOnly<T>(config, (TimeSpan?)(v.From?.TimeOfDay), (TimeSpan?)(v.To?.TimeOfDay)),
                    (null or TimeSpan, null or TimeSpan) v when config.FieldType is FieldType.Time =>
                        new InBetweenInclusiveFilterTimeOnly<T>(config, v.From, v.To),
                    var (@from, to) => new InBetweenInclusiveFilter<T>(config, @from, to)
                };

                filters.Add(filter);
            }

            foreach (var (key, value) in _includingValues)
            {
                var config = Configs.First(c => c.Identifier == key);
                if(value is null)
                    continue;
                filters.Add(new IncludeFilter<T>(config, value));
            }

            return filters;
        }

        public override RenderFragment CreateField(ColumnConfig<T> config)
        {
            return builder =>
            {
                if (config.PropertyType.IsNumeric())
                    CreateFromToFields(builder, config);
                else if (IsFormOfDateTime(config.PropertyType) && config.FieldType is FieldType.AutoDetect or FieldType.DateAndTime)
                    CreateFromToDateTimeFields(builder, config);
                else if (IsFormOfDateTime(config.PropertyType) && config.FieldType == FieldType.Date)
                    CreateFromToDateFields(builder, config);
                else if ((IsFormOfDateTime(config.PropertyType) || config.PropertyType == typeof(TimeSpan?) || config.PropertyType == typeof(TimeSpan)) &&
                         (config.FieldType is FieldType.AutoDetect or FieldType.Time))
                    CreateFromToTimeFields(builder, config);
                else
                    CreateIncludingField(builder, config);
            };
        }

        private bool IsFormOfDateTime(Type type)
        {
            return type == typeof(DateTime?) || type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?);
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
            builder.AddAttribute(LineNumber.Get(), nameof(MudNumericField<int>.Value), readDelegate.DynamicInvoke(config.Identifier, true));

            var callbackType = typeof(EventCallback<>).MakeGenericType(valueType);
            void SaveFromDelegate(dynamic obj) => _fromToValues[config.Identifier] = (obj, _fromToValues[config.Identifier].To);
            dynamic callbackFrom = Activator.CreateInstance(callbackType, this, (Action<dynamic>)SaveFromDelegate);
            builder.AddAttribute(LineNumber.Get(), nameof(MudNumericField<int>.ValueChanged), callbackFrom);

            builder.AddAttribute(LineNumber.Get(), nameof(MudNumericField<int>.Immediate), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudNumericField<int>.Label), FromLabel);
            var callbackRevalidateFromTo = CallbackFactory.Create<KeyboardEventArgs>(this, _ => ValidateFromToPair(config.Identifier));
            builder.AddAttribute(LineNumber.Get(), nameof(MudNumericField<int>.OnKeyUp), callbackRevalidateFromTo);

            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config, true));
            builder.CloseComponent();

            // Create 'To'
            builder.OpenComponent(LineNumber.Get(), fieldType);

            builder.AddAttribute(LineNumber.Get(), nameof(MudNumericField<int>.Value), readDelegate.DynamicInvoke(config.Identifier, false));
            void SaveToDelegate(dynamic obj) => _fromToValues[config.Identifier] = (_fromToValues[config.Identifier].From, obj);
            dynamic callbackTo = Activator.CreateInstance(callbackType, this, (Action<dynamic>)SaveToDelegate);
            builder.AddAttribute(LineNumber.Get(), nameof(MudNumericField<int>.ValueChanged), callbackTo);

            builder.AddAttribute(LineNumber.Get(), nameof(MudNumericField<int>.Immediate), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudNumericField<int>.Validation), CreateValidationDelegate(config, valueType));
            builder.AddAttribute(LineNumber.Get(), nameof(MudNumericField<int>.Label), ToLabel);
            builder.AddAttribute(LineNumber.Get(), nameof(MudNumericField<int>.OnKeyUp), callbackRevalidateFromTo);
            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config, false));
            builder.CloseComponent();
        }

        private TData GetDataFromTo<TData>(Guid id, bool isFrom) => isFrom ? _fromToValues[id].From : _fromToValues[id].To;

        private static Type EnsureNullable(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                ? type
                : typeof(Nullable<>).MakeGenericType(type);
        }

        private void ValidateFromToPair(Guid id)
        {
            if (!_fieldReferencesFromTo.ContainsKey(id)) return;
            _fieldReferencesFromTo[id].From?.Validate();
            _fieldReferencesFromTo[id].To?.Validate();
        }

        private void CreateFieldReference(object o, ColumnConfig<T> config, bool isFrom)
        {
            if (_fieldReferencesFromTo.TryGetValue(config.Identifier, out var value))
            {
                if (value.From is not null && value.To is not null)
                    return;
                _fieldReferencesFromTo[config.Identifier] = isFrom switch
                {
                    true when value.From is null => (o, _fieldReferencesFromTo[config.Identifier].To),
                    false when value.To is null => (_fieldReferencesFromTo[config.Identifier].From, o),
                    _ => _fieldReferencesFromTo[config.Identifier]
                };
            }
            else
            {
                _fieldReferencesFromTo.Add(config.Identifier, isFrom ? (o, null) : (null, o));
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
            CreateDateField(builder, config, true);
            CreateTimeField(builder, config, true);

            // Create 'To'
            CreateDateField(builder, config, false, true);
            CreateTimeField(builder, config, false);
        }

        private void CreateFromToDateFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            _fromToValues.TryAdd(config.Identifier, (null, null));
            CreateDateField(builder, config, true);
            CreateDateField(builder, config, false, true);
        }

        private void CreateFromToTimeFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            _fromToValues.TryAdd(config.Identifier, (null, null));
            CreateTimeField(builder, config, true);
            CreateTimeField(builder, config, false, true);
        }

        private void CreateIncludingField(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            _includingValues.TryAdd(config.Identifier, null);

            builder.OpenComponent(LineNumber.Get(), typeof(MudTextField<string>));
            builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.Value), _includingValues[config.Identifier]);
            var callbackValueChanged = CallbackFactory.Create<string>(this, s => _includingValues[config.Identifier] = string.IsNullOrEmpty(s) ? null : s);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.ValueChanged), callbackValueChanged);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.Label), IncludeLabel);
            builder.CloseComponent();
        }

        private void CreateDateField(RenderTreeBuilder builder, ColumnConfig<T> config, bool isFrom, bool shouldValidate = false)
        {
            DateTime? value = isFrom ? _fromToValues[config.Identifier].From : _fromToValues[config.Identifier].To;

            builder.OpenComponent(LineNumber.Get(), typeof(MudDatePicker));
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.Date), value);
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.DisableToolbar), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.Editable), true);

            var callback = CallbackFactory.Create<DateTime?>(this, d => EditDate(d, config, isFrom));
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.DateChanged), callback);

            if (shouldValidate)
            {
                string Validate(DateTime? date)
                {
                    if (date is null) return null;
                    if (isFrom && ((DateTime?)_fromToValues[config.Identifier].To).HasValue is false) return null;
                    if (isFrom is false && ((DateTime?)_fromToValues[config.Identifier].From).HasValue is false) return null;
                    var compareTo = isFrom
                        ? (DateTime?)_fromToValues[config.Identifier].To.Date
                        : (DateTime?)_fromToValues[config.Identifier].From.Date;
                    return isFrom
                        ? (date <= compareTo ? null : ErrorMessageFromTo)
                        : (date >= compareTo ? null : ErrorMessageFromTo);
                }

                var callbackRevalidateFromTo = CallbackFactory.Create(this, () => ValidateFromToPair(config.Identifier));
                builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.PickerClosed), callbackRevalidateFromTo);
                builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.Validation), (Func<DateTime?, string>)Validate);
            }

            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.PickerActions), (RenderFragment)(builderInternal =>
           {
               builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
               var okCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudDatePicker)FromToField(config, isFrom)).Close());
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), okCallback);
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                   (RenderFragment)(b => b.AddContent(LineNumber.Get(), PickerOKLabel)));
               builderInternal.CloseComponent();
               builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
               var cancelCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudDatePicker)FromToField(config, isFrom)).Close(false));
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), cancelCallback);
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                   (RenderFragment)(b => b.AddContent(LineNumber.Get(), PickerCancelLabel)));
               builderInternal.CloseComponent();
               builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
               var clearCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudDatePicker)FromToField(config, isFrom)).Clear(false));
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), clearCallback);
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                   (RenderFragment)(b => b.AddContent(LineNumber.Get(), PickerClearLabel)));
               builderInternal.CloseComponent();
           }));

            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.Label), isFrom ? FromLabel : ToLabel);
            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config, isFrom));
            builder.CloseComponent();
        }

        /// <summary>
        /// Returns reference to 'from' or 'to' field / picker pointed to by given <paramref name="config"/>.
        /// </summary>
        /// <param name="config"><see cref="_fieldReferencesFromTo"/> index.</param>
        /// <param name="isFrom">Choose if either 'from' or 'to' value should be returned.</param>
        /// <returns>If <paramref name="isFrom"/> is <see langword="true"/>, than a 'from' value from <see cref="_fieldReferencesFromTo"/> with index
        /// corresponding to given <paramref name="config"/> is returned. Otherwise, 'to' part is returned.</returns>
        private dynamic FromToField(ColumnConfig<T> config, bool isFrom) => isFrom
            ? _fieldReferencesFromTo[config.Identifier].From
            : _fieldReferencesFromTo[config.Identifier].To;

        /// <summary>
        /// Returns data from 'from' or 'to' field pointed to by given <paramref name="config"/>.
        /// </summary>
        /// <param name="config"><see cref="_fromToValues"/> index.</param>
        /// <param name="isFrom">Choose if either 'from' or 'to' value should be returned.</param>
        /// <returns>If <paramref name="isFrom"/> is <see langword="true"/>, than a 'from' value from <see cref="_fromToValues"/> with index
        /// corresponding to given <paramref name="config"/> is returned. Otherwise, 'to' part is returned.</returns>
        private dynamic FromToFieldData(ColumnConfig<T> config, bool isFrom) => isFrom
            ? _fromToValues[config.Identifier].From
            : _fromToValues[config.Identifier].To;

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

        private void CreateTimeField(RenderTreeBuilder builder, ColumnConfig<T> config, bool isFrom, bool shouldValidate = false)
        {
            var value = isFrom ? _fromToValues[config.Identifier].From : _fromToValues[config.Identifier].To;
            TimeSpan? time = null;
            var editingCallback = EventCallback<TimeSpan?>.Empty;
            Func<TimeSpan?, string> validationFunc = null;

            if (config.PropertyType == typeof(DateTime?) || config.PropertyType == typeof(DateTime) ||
                config.PropertyType == typeof(DateTimeOffset?) || config.PropertyType == typeof(DateTimeOffset))
            {
                var data = (value is DateTimeOffset offset ? offset.LocalDateTime : value) as DateTime?;
                time = data.HasValue switch
                {
                    true => data.Value.TimeOfDay,
                    false => null
                };

                editingCallback = CallbackFactory.Create<TimeSpan?>(this, d => EditTimeInDate(d, config, isFrom));
                validationFunc = t =>
                {
                    if (t is null || ((DateTime?)FromToFieldData(config, isFrom)).HasValue is false) return null;

                    var compareTo = (TimeSpan?)(FromToFieldData(config, isFrom).TimeOfDay);
                    return isFrom
                        ? (t <= compareTo ? null : ErrorMessageFromTo)
                        : (t >= compareTo ? null : ErrorMessageFromTo);
                };
            }
            else if (config.PropertyType == typeof(TimeSpan?) || config.PropertyType == typeof(TimeSpan))
            {
                time = value;
                editingCallback = CallbackFactory.Create<TimeSpan?>(this, d => EditTime(d, config, isFrom));
                validationFunc = t =>
                {
                    if (t is null) return null;
                    var compareTo = (TimeSpan?)FromToFieldData(config, isFrom);
                    return isFrom
                        ? (t <= compareTo ? null : ErrorMessageFromTo)
                        : (t >= compareTo ? null : ErrorMessageFromTo);
                };
            }

            builder.OpenComponent(LineNumber.Get(), typeof(MudTimePicker));
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.DisableToolbar), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.Editable), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.Time), time);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.TimeChanged), editingCallback);

            if (shouldValidate)
            {
                var callbackRevalidateFromTo = CallbackFactory.Create(this, () => ValidateFromToPair(config.Identifier));
                builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.Validation), validationFunc);
                builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.PickerClosed), callbackRevalidateFromTo);
            }

            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.PickerActions), (RenderFragment)(builderInternal =>
           {
               builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
               var okCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudTimePicker)FromToField(config, isFrom)).Close());
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), okCallback);
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                   (RenderFragment)(b => b.AddContent(LineNumber.Get(), PickerOKLabel)));
               builderInternal.CloseComponent();
               builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
               var cancelCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudTimePicker)FromToField(config, isFrom)).Close(false));
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), cancelCallback);
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                   (RenderFragment)(b => b.AddContent(LineNumber.Get(), PickerCancelLabel)));
               builderInternal.CloseComponent();
               builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
               var clearCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudTimePicker)FromToField(config, isFrom)).Clear(false));
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), clearCallback);
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                   (RenderFragment)(b => b.AddContent(LineNumber.Get(), PickerClearLabel)));
               builderInternal.CloseComponent();
           }));
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.Label), isFrom ? FromLabel : ToLabel);
            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config, isFrom));

            builder.CloseComponent();
        }

        private void EditTimeInDate(TimeSpan? newTime, ColumnConfig<T> config, bool isFrom)
        {
            if (FromToFieldData(config, isFrom) is DateTime oldDate)
            {
                var newDate = oldDate.Date + newTime;
                if (newDate == oldDate) return;
                if (isFrom)
                {
                    _fromToValues[config.Identifier] = (newDate, _fromToValues[config.Identifier].To);
                }
                else
                {
                    _fromToValues[config.Identifier] = (_fromToValues[config.Identifier].From, newDate);
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
            var source = FromToFieldData(config, isFrom) as TimeSpan?;

            if (time != source)
            {
                if (isFrom)
                {
                    _fromToValues[config.Identifier] = (time, _fromToValues[config.Identifier].To);
                }
                else
                {
                    _fromToValues[config.Identifier] = (_fromToValues[config.Identifier].From, time);
                }
            }
        }
    }
}