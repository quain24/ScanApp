using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;
using ScanApp.Common.Extensions;
using ScanApp.Common.Helpers;
using ScanApp.Components.Common.ScanAppTable.Options;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;

namespace ScanApp.Components.Common.AltTableTest
{
    public partial class EditFieldCreator<T> : FieldCreatorBase<T>
    {
        [Parameter]
        public T TargetItem { get; set; }

        [Parameter]
        public EventCallback<T> TargetItemChanged { get; set; }

        private readonly Dictionary<Guid, dynamic> _fieldReferences = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (TargetItem is null)
                throw new ArgumentNullException(nameof(TargetItem), "Target Item must be set (You can use @bind-TargetItem)");
        }

        public override RenderFragment CreateField(ColumnConfig<T> config)
        {
            return builder =>
            {
                if ((config.PropertyType == typeof(DateTime?) && config.FieldType == FieldType.AutoDetect) ||
                    config.FieldType is FieldType.Date or FieldType.DateAndTime)
                    CreateDateFields(builder, config);
                if ((config.PropertyType == typeof(DateTime?) && config.FieldType == FieldType.AutoDetect) ||
                    (config.PropertyType == typeof(TimeSpan?) && config.FieldType == FieldType.AutoDetect) ||
                    config.FieldType is FieldType.Time or FieldType.DateAndTime)
                    CreateTimeFields(builder, config);
                else if (config.FieldType == FieldType.PlainText || (config.PropertyType != typeof(DateTime?) && config.PropertyType != typeof(TimeSpan?)))
                    CreateTextField(builder, config);
            };
        }

        private void CreateTextField(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            // Type of mud blazor text field (int, string, etc)
            var textFieldType = typeof(MudTextField<>).MakeGenericType(config.PropertyType);
            // todo add custom edit converter - mud blazor mechanism to text fields - put it in Column Config
            // Start creating text field
            builder.OpenComponent(LineNumber.Get(), textFieldType);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.Value), config.GetValueFrom(TargetItem) as object);

            // Set callback for edit action
            var callbackType = typeof(EventCallback<>).MakeGenericType(config.PropertyType);
            async Task EditDelegate(dynamic obj)
            {
                ColumnConfigExtensions.SetValue(config, TargetItem, obj);
                await TargetItemChanged.InvokeAsync(TargetItem);
            }

            dynamic callback = Activator.CreateInstance(callbackType, this, (Func<dynamic, Task>)EditDelegate);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.ValueChanged), callback);

            // Set up corresponding validator
            if (Validators.TryGetValue(config, out var validatorDelegate))
                builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.Validation), validatorDelegate);

            // Set common options
            builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.Immediate), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTextField<string>.Disabled), !config.IsEditable);

            // Finish component
            builder.CloseComponent();
        }

        private void CreateDateFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            builder.OpenComponent(LineNumber.Get(), typeof(MudDatePicker));

            var editCallback = CallbackFactory.Create<DateTime?>(this, d => EditDate(d, config));
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.Date), config.GetValueFrom(TargetItem) as DateTime?);
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.DateChanged), editCallback);
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.DisableToolbar), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.Editable), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.Disabled), !config.IsEditable);
            if (Validators.TryGetValue(config, out var validatorDelegate))
                builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.Validation), validatorDelegate);
            builder.AddAttribute(LineNumber.Get(), nameof(MudDatePicker.PickerActions), (RenderFragment) (builderInternal =>
            {
                builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
                var okCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudDatePicker) _fieldReferences[config.Identifier]).Close());
                builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), okCallback);
                builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                    (RenderFragment) (b => b.AddContent(LineNumber.Get(), "Ok")));
                builderInternal.CloseComponent();
                builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
                var cancelCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudDatePicker) _fieldReferences[config.Identifier]).Close(false));
                builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), cancelCallback);
                builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                    (RenderFragment) (b => b.AddContent(LineNumber.Get(), "Cancel")));
                builderInternal.CloseComponent();
            }));

            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config));
            builder.CloseComponent();
        }

        private void CreateTimeFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            TimeSpan? time = null;
            var callback = EventCallback<TimeSpan?>.Empty;
            if (config.PropertyType == typeof(DateTime?) || config.PropertyType == typeof(DateTime))
            {
                var data = config.GetValueFrom(TargetItem) as DateTime?;
                time = data.HasValue switch
                {
                    true => data.Value.TimeOfDay,
                    false => null
                };

                callback = CallbackFactory.Create<TimeSpan?>(this, d => EditTimeInDate(d, config));
            }

            if (config.PropertyType == typeof(TimeSpan?) || config.PropertyType == typeof(TimeSpan))
            {
                time = config.GetValueFrom(TargetItem) as TimeSpan?;
                callback = CallbackFactory.Create<TimeSpan?>(this, d => EditTime(d, config));
            }

            builder.OpenComponent(LineNumber.Get(), typeof(MudTimePicker));

            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.Time), time);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.TimeChanged), callback);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.DisableToolbar), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.Editable), true);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.Disabled), !config.IsEditable);
            if (Validators.TryGetValue(config, out var validatorDelegate))
                builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.Validation), validatorDelegate);
            builder.AddAttribute(LineNumber.Get(), nameof(MudTimePicker.PickerActions), (RenderFragment) (builderInternal =>
            {
                builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
                var okCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudTimePicker) _fieldReferences[config.Identifier]).Close());
                builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), okCallback);
                builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                    (RenderFragment) (b => b.AddContent(LineNumber.Get(), "Ok")));
                builderInternal.CloseComponent();
                builderInternal.OpenComponent(LineNumber.Get(), typeof(MudButton));
                var cancelCallback = CallbackFactory.Create<MouseEventArgs>(this, _ => ((MudTimePicker) _fieldReferences[config.Identifier]).Close(false));
                builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.OnClick), cancelCallback);
                builderInternal.AddAttribute(LineNumber.Get(), nameof(MudButton.ChildContent),
                    (RenderFragment) (b => b.AddContent(LineNumber.Get(), "Cancel")));
                builderInternal.CloseComponent();
            }));

            builder.AddComponentReferenceCapture(LineNumber.Get(), o => CreateFieldReference(o, config));
            builder.CloseComponent();
        }

        private void CreateFieldReference(object o, ColumnConfig<T> config)
        {
            if (_fieldReferences.TryGetValue(config.Identifier, out var value))
            {
                if (value is not null)
                    return;
                _fieldReferences[config.Identifier] = o;
            }
            else
            {
                _fieldReferences.Add(config.Identifier, o);
            }
        }

        private async Task EditDate(DateTime? date, ColumnConfig<T> config)
        {
            var oldDate = config.GetValueFrom(TargetItem) as DateTime?;

            if (config.PropertyType == typeof(DateTime) && date.HasValue is false)
            {
                config.SetValue(TargetItem, DateTime.MinValue + oldDate.Value.TimeOfDay);
            }
            else
            {
                if (date != oldDate && oldDate.HasValue)
                {
                    date += oldDate.Value.TimeOfDay;
                }
                config.SetValue(TargetItem, date);
            }
            await TargetItemChanged.InvokeAsync(TargetItem);
        }

        private async Task EditTime(TimeSpan? time, ColumnConfig<T> config)
        {
            if (config.PropertyType == typeof(TimeSpan) && time.HasValue is false)
            {
                config.SetValue(TargetItem, TimeSpan.Zero);
                await TargetItemChanged.InvokeAsync(TargetItem);
            }
            else if (time != config.GetValueFrom(TargetItem) as TimeSpan?)
            {
                config.SetValue(TargetItem, time);
                await TargetItemChanged.InvokeAsync(TargetItem);
            }
        }

        private async Task EditTimeInDate(TimeSpan? newTime, ColumnConfig<T> config)
        {
            if (config.GetValueFrom(TargetItem) is DateTime oldDate)
            {
                var newDate = oldDate.Date + newTime;
                if (newDate != oldDate)
                {
                    config.SetValue(TargetItem, newDate);
                    await TargetItemChanged.InvokeAsync(TargetItem);
                }
            }
            else if (newTime.HasValue)
            {
                config.SetValue(TargetItem, DateTime.MinValue + newTime.Value);
                await TargetItemChanged.InvokeAsync(TargetItem);
            }
        }
    }
}