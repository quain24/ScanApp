﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using ScanApp.Common.Extensions;
using ScanApp.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Components.Common.Table.Dialogs
{
    public partial class EditFieldCreator<T> : FieldCreatorBase<T>
    {
        /// <summary>
        /// Gets or sets item that will be edited by fields created by this instance.
        /// </summary>
        /// <value>Item to be edited if set.</value>
        [Parameter] public T TargetItem { get; set; }

        /// <summary>
        /// Callback is called when edited item has been changed and returns edited item.
        /// </summary>
        /// <value>Callback.</value>
        [Parameter] public EventCallback<T> TargetItemChanged { get; set; }

        private readonly Dictionary<ColumnConfig<T>, dynamic> _fieldReferences = new();
        private readonly Dictionary<ColumnConfig<T>, MudTimePicker> _fieldReferencesForTimePartInDateTime = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (TargetItem is null)
                throw new ArgumentNullException(nameof(TargetItem), "Target Item must be set (You can use @bind-TargetItem)");
        }

        public void ExpandInvalidPanels()
        {
            var invalidPanels = _fieldReferences
                .Where(kvp => kvp.Value?.ValidationErrors?.Count != 0)
                .Select(p => p.Key)
                .Concat(_fieldReferencesForTimePartInDateTime.Where(kvp => kvp.Value?.ValidationErrors?.Count != 0)
                    .Select(p => p.Key))
                .Distinct();

            foreach (var key in invalidPanels)
            {
                Panels[key].Expand(false);
            }

            StateHasChanged();
        }

        protected override RenderFragment CreateField(ColumnConfig<T> config)
        {
            return builder =>
            {
                if ((config.PropertyType == typeof(DateTime?) || config.PropertyType == typeof(DateTime)) &&
                    config.FieldType is FieldType.Date or FieldType.DateAndTime or FieldType.AutoDetect)
                    CreateDateFields(builder, config);
                if ((config.PropertyType == typeof(DateTime?) || config.PropertyType == typeof(DateTime) ||
                     config.PropertyType == typeof(TimeSpan?) || config.PropertyType == typeof(TimeSpan)) &&
                     config.FieldType is FieldType.AutoDetect or FieldType.Time or FieldType.DateAndTime)
                    CreateTimeFields(builder, config);
                else if (config.FieldType == FieldType.PlainText ||
                        (config.PropertyType != typeof(DateTime?) && config.PropertyType != typeof(TimeSpan?) &&
                         config.PropertyType != typeof(DateTime) && config.PropertyType != typeof(TimeSpan)))
                    CreateTextField(builder, config);
            };
        }

        private void CreateTextField(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            // Type of mud blazor text field (int, string, etc)
            var textFieldType = typeof(MudTextField<>).MakeGenericType(config.PropertyType);

            // Start creating text field
            builder.OpenComponent(LineNumber.Get(), textFieldType);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.Value), config.GetValueFrom(TargetItem) as object);

            // Set callback for edit action
            var callbackType = typeof(EventCallback<>).MakeGenericType(config.PropertyType);
            async Task EditDelegate(dynamic obj)
            {
                TargetItem = config.SetValue(TargetItem, obj);
                await TargetItemChanged.InvokeAsync(TargetItem);
            }

            dynamic callback = Activator.CreateInstance(callbackType, this, (Func<dynamic, Task>)EditDelegate);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.ValueChanged), callback);

            // Set up corresponding validator
            if (Validators.TryGetValue(config, out var validatorDelegate))
                builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.Validation), validatorDelegate);

            // Apply custom converter for get / set value if there is one (from text/int/etc to object and vice-versa)
            if (config.Converter is not null)
                builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.Converter), config.Converter);

            // Set common options
            builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.Immediate), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.Disabled), !config.IsEditable);

            // Add field reference to collection.
            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config));

            // Finish component
            builder.CloseComponent();
        }

        private void CreateDateFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            var editCallback = CallbackFactory.Create<DateTime?>(this, d => EditDate(d, config));

            builder.OpenComponent(LineNumber.Get(), typeof(MudDatePicker));
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.Date), config.GetValueFrom(TargetItem) as DateTime?);
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.DateChanged), editCallback);
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.DisableToolbar), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.Editable), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.Disabled), !config.IsEditable);
            if (config.Converter is not null)
                builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.Converter), config.Converter);
            if (Validators.TryGetValue(config, out var validatorDelegate))
                builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.Validation), validatorDelegate);
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.PickerActions), (RenderFragment)(builderInternal =>
           {
               builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
               var okCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudDatePicker)_fieldReferences[config]).Close());
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), okCallback);
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                   (RenderFragment)(b => b.AddContent(LineNumber.Get(), PickerOKLabel)));
               builderInternal.CloseComponent();
               builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
               var cancelCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudDatePicker)_fieldReferences[config]).Close(false));
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), cancelCallback);
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                   (RenderFragment)(b => b.AddContent(LineNumber.Get(), PickerCancelLabel)));
               builderInternal.CloseComponent();
               if (Nullable.GetUnderlyingType(config.PropertyType) is not null)
               {
                   builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
                   var clearCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudDatePicker)_fieldReferences[config]).Clear(false));
                   builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), clearCallback);
                   builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.Style), "mr-auto align-self-start");
                   builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                       (RenderFragment)(b => b.AddContent(LineNumber.Get(), PickerClearLabel)));
                   builderInternal.CloseComponent();
               }
           }));

            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config));
            builder.CloseComponent();
        }

        private void CreateFieldReference(object o, ColumnConfig<T> config)
        {
            if (_fieldReferences.TryGetValue(config, out var value))
            {
                if (value is not null)
                    return;
                _fieldReferences[config] = o;
            }
            else
            {
                _fieldReferences.Add(config, o);
            }
        }

        private void CreateTimeFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            TimeSpan? time = null;
            var callback = EventCallback<TimeSpan?>.Empty;
            var clearCallback = EventCallback<MouseEventArgs>.Empty;
            if (config.PropertyType == typeof(DateTime?) || config.PropertyType == typeof(DateTime))
            {
                var data = config.GetValueFrom(TargetItem) as DateTime?;
                time = data.HasValue switch
                {
                    true => data.Value.TimeOfDay,
                    false => null
                };

                callback = CallbackFactory.Create<TimeSpan?>(this, d => EditTimeInDate(d, config));
                clearCallback = CallbackFactory.Create<MouseEventArgs>(this, async _ => await EditTimeInDate(null, config));
            }

            if (config.PropertyType == typeof(TimeSpan?) || config.PropertyType == typeof(TimeSpan))
            {
                time = config.GetValueFrom(TargetItem) as TimeSpan?;
                callback = CallbackFactory.Create<TimeSpan?>(this, d => EditTime(d, config));
                clearCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudTimePicker)_fieldReferences[config]).Clear(false));
            }

            builder.OpenComponent(LineNumber.Get(), typeof(MudTimePicker));

            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.Time), time);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.TimeChanged), callback);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.DisableToolbar), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.Editable), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.Disabled), !config.IsEditable);
            if (config.Converter is not null)
                builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.Converter), config.Converter);
            if (Validators.TryGetValue(config, out var validatorDelegate))
                builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.Validation), validatorDelegate);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.PickerActions), (RenderFragment)(builderInternal =>
           {
               builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
               var okCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudTimePicker)_fieldReferencesForTimePartInDateTime[config]).Close());
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), okCallback);
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                   (RenderFragment)(b => b.AddContent(LineNumber.Get(), PickerOKLabel)));
               builderInternal.CloseComponent();
               builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
               var cancelCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudTimePicker)_fieldReferencesForTimePartInDateTime[config]).Close(false));
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), cancelCallback);
               builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                   (RenderFragment)(b => b.AddContent(LineNumber.Get(), PickerCancelLabel)));
               builderInternal.CloseComponent();
               if (Nullable.GetUnderlyingType(config.PropertyType) is not null)
               {
                   builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
                   builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), clearCallback);
                   builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.Style), "mr-auto align-self-start");
                   builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                       (RenderFragment)(b => b.AddContent(LineNumber.Get(), PickerClearLabel)));
                   builderInternal.CloseComponent();
               }
           }));

            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReferenceForTimeInDateTime(o as MudTimePicker, config));
            builder.CloseComponent();
        }

        private void CreateFieldReferenceForTimeInDateTime(MudTimePicker o, ColumnConfig<T> config)
        {
            if (_fieldReferencesForTimePartInDateTime.TryGetValue(config, out var value))
            {
                if (value is not null)
                    return;
                _fieldReferencesForTimePartInDateTime[config] = o;
            }
            else
            {
                _fieldReferencesForTimePartInDateTime.Add(config, o);
            }
        }

        private async Task EditDate(DateTime? date, ColumnConfig<T> config)
        {
            var oldDate = config.GetValueFrom(TargetItem) as DateTime?;

            if (config.PropertyType == typeof(DateTime) && date.HasValue is false)
            {
                TargetItem = config.SetValue(TargetItem, DateTime.MinValue + oldDate.Value.TimeOfDay);
            }
            else
            {
                if (date != oldDate && oldDate.HasValue)
                {
                    date += oldDate.Value.TimeOfDay;
                }
                TargetItem = config.SetValue(TargetItem, date);
            }
            await TargetItemChanged.InvokeAsync(TargetItem);
        }

        private async Task EditTime(TimeSpan? time, ColumnConfig<T> config)
        {
            if (time.HasValue)
            {
                if (time != config.GetValueFrom(TargetItem) as TimeSpan?)
                {
                    TargetItem = config.SetValue(TargetItem, time.Value);
                    await TargetItemChanged.InvokeAsync(TargetItem);
                }
                return;
            }

            if (Nullable.GetUnderlyingType(config.PropertyType) is null && TimeSpan.Zero != (TimeSpan)config.GetValueFrom(TargetItem))
            {
                TargetItem = config.SetValue(TargetItem, TimeSpan.Zero);
                await TargetItemChanged.InvokeAsync(TargetItem);
            }
            else if (config.GetValueFrom(TargetItem) is not null)
            {
                TargetItem = config.SetValue(TargetItem, null);
                await TargetItemChanged.InvokeAsync(TargetItem);
            }
        }

        private async Task EditTimeInDate(TimeSpan? newTime, ColumnConfig<T> config)
        {
            if (config.GetValueFrom(TargetItem) is DateTime oldDate)
            {
                var newDate = oldDate.Date + (newTime ?? TimeSpan.Zero);
                if (newDate != oldDate)
                {
                    TargetItem = config.SetValue(TargetItem, newDate);
                    await TargetItemChanged.InvokeAsync(TargetItem);
                }
            }
            else if (newTime.HasValue)
            {
                TargetItem = config.SetValue(TargetItem, DateTime.MinValue + newTime.Value);
                await TargetItemChanged.InvokeAsync(TargetItem);
            }
        }
    }
}